using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Exceptions;
using MyBuildingBlock.Models;
using MyBuildingBlock.Security.Controllers;
using MyBuildingBlock.Security.Models;
using MyBuildingBlock.Security.Services;
using GCSS_Survy.Data;
using System.Security.Claims;
using System.Text.Json;

namespace GCSS_Survy.Services.Security
{
    [ApiController]
    [Route($"{BaseConstants.BASE_URL}/[controller]/v1")]
    [EnableRateLimiting("Authentication")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[BasePermission(true)]

    public class SecureAuthController : BaseAuthenticationController<Users, Branches, Sessions>
    {
        //private readonly ITokenService _tokenService;
        //private readonly ISecurityService _securityService;
        //private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public SecureAuthController(
            ApplicationDbContext context,
            ILogger<BaseAuthenticationController<Users, Branches, Sessions>> logger,
            IMapper mapper,
            IMediator mediator,
            ITokenService tokenService,
            ISecurityService securityService,
            IAuthenticationService<Users, Branches, Sessions> authService, IConfiguration configuration
            ,IServiceProvider serviceProvider)
            : base(tokenService, securityService, authService, logger)
        {
            //_tokenService = tokenService;
            //_securityService = securityService;
            //_authService = authService;
            //_config = configuration;
            _context = context;
            logger.LogInformation("SecureAuthController initialized successfully");
        }

        /// <summary>
        /// Helper method to convert ReturnResult<T> to IActionResult
        /// Extracts Code property and sets HTTP status code accordingly
        /// </summary>
        protected IActionResult ToResult<T>(ReturnResult<T> result)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var statusCode = result.Code;
            if (statusCode > 500)
                statusCode = 500;
            if (statusCode < 200)
                statusCode = 200;

            Response.StatusCode = statusCode;
            return new JsonResult(result, options)
            {
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Login endpoint - returns ReturnResult<LoginResponse>
        /// </summary>
        public override async Task<IActionResult> LoginAsync([FromBody] SecureLoginRequest request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                // Validate IP address
                if (!await _securityService.IsIpAddressAllowedAsync(ipAddress))
                {
                    await _securityService.LogSecurityEventAsync(
                        SecurityEventType.UnauthorizedAccess,
                        request.Username, ipAddress, "IP address not allowed");
                    
                    var result = new ReturnResult<LoginResponse>
                    {
                        Code = 401,
                        Status = "Failed",
                        Title = "Access denied",
                        Message = "IP address not allowed"
                    };
                    return ToResult(result);
                }

                // Check account lockout
                if (await _securityService.IsAccountLockedOutAsync(request.Username))
                {
                    var remainingTime = await _securityService.GetRemainingLockoutTimeAsync(request.Username);
                    var result = new ReturnResult<LoginResponse>
                    {
                        Code = 400,
                        Status = "Failed",
                        Title = "Account locked",
                        Message = $"Account locked. {remainingTime?.TotalMinutes:F0} minutes remaining"
                    };
                    return ToResult(result);
                }

                // Authenticate
                var authResult = await _authService.AuthenticateAsync(request, ipAddress);

                if (!authResult.IsSuccess)
                {
                    await _securityService.RecordFailedLoginAttemptAsync(request.Username, ipAddress);
                    await _securityService.LogSecurityEventAsync(
                        SecurityEventType.LoginFailure,
                        request.Username, ipAddress, authResult.ErrorMessage ?? "Invalid credentials");

                    var result = new ReturnResult<LoginResponse>
                    {
                        Code = 401,
                        Status = "Failed",
                        Title = "Authentication failed",
                        Message = authResult.ErrorMessage ?? "Invalid credentials",
                        Errors = authResult.Error != null ? new Dictionary<string, string> { { "error", authResult.Error } } : null
                    };
                    return ToResult(result);
                }

                // Reset failed attempts
                await _securityService.ResetFailedLoginAttemptsAsync(request.Username);

                // Generate tokens
                var accessToken = await _tokenService.GenerateAccessTokenAsync(authResult);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

                // Store refresh token
                await StoreRefreshTokenAsync(authResult.UserId, refreshToken);

                // Log successful login
                await _securityService.LogSecurityEventAsync(
                    SecurityEventType.LoginSuccess,
                    request.Username, ipAddress);

                var loginResponse = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = 300 * 60, // 5 hours in seconds
                    BranchId = authResult.BranchId ?? 0,
                    UserId = authResult.UserId,
                    SessionId = authResult.SessionId,
                    RuleId = authResult.RuleId,
                    BranchName = request.BranchName
                };

                var successResult = new ReturnResult<LoginResponse>(loginResponse)
                {
                    Code = 200,
                    Status = "Success"
                };
                return ToResult(successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user {Username}", request.Username);
                var result = new ReturnResult<LoginResponse>
                {
                    Code = 500,
                    Status = "Failed",
                    Title = "Internal Server Error",
                    Message = "An error occurred during login"
                };
                return ToResult(result);
            }
        }

        /// <summary>
        /// Refresh token endpoint - returns ReturnResult<TokenResponse>
        /// </summary>
        public override async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var tokenResponse = await _tokenService.RefreshTokenAsync(request.RefreshToken);

                if (tokenResponse == null)
                {
                    var result = new ReturnResult<TokenResponse>
                    {
                        Code = 401,
                        Status = "Failed",
                        Title = "Invalid refresh token",
                        Message = "Invalid refresh token"
                    };
                    return ToResult(result);
                }

                var successResult = new ReturnResult<TokenResponse>(tokenResponse)
                {
                    Code = 200,
                    Status = "Success"
                };
                return ToResult(successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh error");
                var result = new ReturnResult<TokenResponse>
                {
                    Code = 500,
                    Status = "Failed",
                    Title = "Internal Server Error",
                    Message = "An error occurred during token refresh"
                };
                return ToResult(result);
            }
        }

        /// <summary>
        /// Logout endpoint - returns ReturnResult<LogoutResponse>
        /// </summary>
        public override async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest? request = null)
        {
            try
            {
                // Extract token from header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = _tokenService.GetUserIdFromToken(token);

                if (userId.HasValue)
                {
                    // Blacklist the access token
                    var jti = GetJtiFromToken(token);
                    if (!string.IsNullOrEmpty(jti))
                    {
                        await _tokenService.BlacklistTokenAsync(jti, DateTime.Now.AddHours(5));
                    }

                    // Revoke refresh token
                    if (request != null && !string.IsNullOrEmpty(request.RefreshToken))
                    {
                        await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
                    }

                    // End session
                    var sessionIdClaim = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
                    if (long.TryParse(sessionIdClaim, out var sessionId))
                    {
                        await _authService.EndSessionAsync(sessionId);
                    }
                }

                var logoutResponse = new LogoutResponse
                {
                    Message = "Logged out successfully"
                };

                var result = new ReturnResult<LogoutResponse>(logoutResponse)
                {
                    Code = 200,
                    Status = "Success"
                };
                return ToResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                var result = new ReturnResult<LogoutResponse>
                {
                    Code = 500,
                    Status = "Failed",
                    Title = "Internal Server Error",
                    Message = "An error occurred during logout"
                };
                return ToResult(result);
            }
        }

        /// <summary>
        /// Change password endpoint - returns ReturnResult<ChangePasswordResponse>
        /// </summary>
        public override async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = request.UserId;

                // Validate new password
                var validation = await _securityService.ValidatePasswordAsync(request.NewPassword);
                if (!validation.IsValid)
                {
                    var errors = new Dictionary<string, string>();
                    if (validation.Errors != null)
                    {
                        foreach (var error in validation.Errors)
                        {
                            errors[error] = error;
                        }
                    }

                    var result = new ReturnResult<ChangePasswordResponse>
                    {
                        Code = 400,
                        Status = "Failed",
                        Title = "Password validation failed",
                        Message = "كلمة المرور لا توافق المتطلبات المشروط",
                        Errors = errors
                    };
                    return ToResult(result);
                }

                // Hash and update password
                var hashedPassword = _securityService.HashPassword(request.NewPassword);
                await _authService.UpdatePasswordAsync(userId, hashedPassword);

                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                await _securityService.LogSecurityEventAsync(
                    SecurityEventType.PasswordChanged,
                    username, ipAddress);

                var changePasswordResponse = new ChangePasswordResponse
                {
                    Message = "Password changed successfully"
                };

                var successResult = new ReturnResult<ChangePasswordResponse>(changePasswordResponse)
                {
                    Code = 200,
                    Status = "Success"
                };
                return ToResult(successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change password error");
                var result = new ReturnResult<ChangePasswordResponse>
                {
                    Code = 500,
                    Status = "Failed",
                    Title = "Internal Server Error",
                    Message = "An error occurred during password change"
                };
                return ToResult(result);
            }
        }

        /// <summary>
        /// Secure change password endpoint - returns ReturnResult<ChangePasswordResponse>
        /// </summary>
        public override async Task<IActionResult> SecureChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    var result = new ReturnResult<ChangePasswordResponse>
                    {
                        Code = 401,
                        Status = "Failed",
                        Title = "Unauthorized",
                        Message = "Invalid user"
                    };
                    return ToResult(result);
                }

                // Validate current password
                if (!await _authService.ValidatePasswordAsync(userId, request.CurrentPassword))
                {
                    var result = new ReturnResult<ChangePasswordResponse>
                    {
                        Code = 400,
                        Status = "Failed",
                        Title = "Validation failed",
                        Message = "Current password is incorrect"
                    };
                    return ToResult(result);
                }

                // Validate new password
                var validation = await _securityService.ValidatePasswordAsync(request.NewPassword);
                if (!validation.IsValid)
                {
                    var errors = new Dictionary<string, string>();
                    if (validation.Errors != null)
                    {
                        foreach (var error in validation.Errors)
                        {
                            errors[error] = error;
                        }
                    }

                    var result = new ReturnResult<ChangePasswordResponse>
                    {
                        Code = 400,
                        Status = "Failed",
                        Title = "Password validation failed",
                        Message = "كلمة المرور لا توافق المتطلبات المشروط",
                        Errors = errors
                    };
                    return ToResult(result);
                }

                // Hash and update password
                var hashedPassword = _securityService.HashPassword(request.NewPassword);
                await _authService.UpdatePasswordAsync(userId, hashedPassword);

                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                await _securityService.LogSecurityEventAsync(
                    SecurityEventType.PasswordChanged,
                    username, ipAddress);

                var changePasswordResponse = new ChangePasswordResponse
                {
                    Message = "Password changed successfully"
                };

                var successResult = new ReturnResult<ChangePasswordResponse>(changePasswordResponse)
                {
                    Code = 200,
                    Status = "Success"
                };
                return ToResult(successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Secure change password error");
                var result = new ReturnResult<ChangePasswordResponse>
                {
                    Code = 500,
                    Status = "Failed",
                    Title = "Internal Server Error",
                    Message = "An error occurred during password change"
                };
                return ToResult(result);
            }
        }
    }

}
