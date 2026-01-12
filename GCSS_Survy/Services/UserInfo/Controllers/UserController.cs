using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Abstracts;
using MyBuildingBlock.Attributes;
using MyBuildingBlock.Exceptions;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Models;
using MyBuildingBlock.PermissionManagement.Attributes;
using MyBuildingBlock.PermissionManagement.Dtos;
using MyBuildingBlock.Security.Data;
using MyBuildingBlock.Security.Services;
using MyBuildingBlock.UserInfo.Contracts;
using MyBuildingBlock.UserInfo.Dtos;
using MyBuildingBlock.UserInfo.Features.LockingUserAccount;
using MyBuildingBlock.Web;
using GCSS_Survy.Data;

namespace GCSS_Survy.Services.UserInfo.Controllers
{

    [Route($"{BaseConstants.BASE_URL}/User/v1")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [AdminOnly]
    [AdminBranchOnly]
    [BasePermission(true)]
    public class UserController : ControllerBase<UserService<ApplicationDbContext>, AddUserDto, UpdateUserDto, UserResult, int, ApplicationDbContext>
    {
        private readonly ISecurityService _securityService;
        public UserController(ILogger<UserController> logger, IServiceProvider sp, ILookupMapper lookupMapper, ApplicationDbContext context, IMapper mapper)
            : base(context: context, logger, mapper, sp, lookupMapper)
        {
            _securityService = sp.GetRequiredService<ISecurityService>();
        }

       
        [HttpPost]
        [Route("lock")]
        [ActionPermission("lock", "اقفال/فتح حساب مستخدم", MyBuildingBlock.PermissionManagement.Dtos.PermissionConstants.LOCK_ACCOUNT_ICON)]
        public async Task<IActionResult> LockUserAccount([FromBody] LockUserAccount request)
        {
           
            request.userContext = GetUserContext();
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPost]
        [Route("reset-pwrd")]
        public async Task<IActionResult> ResetPwrdAsync([FromBody] ResetPasswordDto request)
        {
            if (request == null)
                return NotFound();
            request.userContext = GetUserContext();
            if (request.userContext.RoleId != 1)
                throw new CustomValidationException("لا يمكنك القيام بهذه العملية");
            if (request.userId == 1)
                throw new CustomValidationException("لا يمكن تهيئة كلمة المرور لمدير النظام");

            var user = await _context.Set<Users>().Where(f => f.ID == request.userId).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.RuleID == 1 && request.userContext.UserId != 1)
                    throw new CustomValidationException($"المستخدم {user.Fname ?? user.UserName} من مدراء النظام لذلك لا يمكن اعادة تهيئة كلمة المرور له الا من قبل مدير النظام");

                user.Password = _securityService.HashPassword(request.newPwrd);
                user.MustChangePassword = true;
                user.ChangePasswordRequiredBefore = DateTime.Now.AddHours(48);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }


        public record ResetPasswordDto
        {
            public int userId { get; set; }
            public string newPwrd { get; set; }
            public UserContext userContext { get; set; }
        }

    }

}
