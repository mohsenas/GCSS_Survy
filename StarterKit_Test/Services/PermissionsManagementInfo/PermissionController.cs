using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Attributes;
using MyBuildingBlock.Exceptions;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Models;
using MyBuildingBlock.PermissionManagement.Attributes;
using MyBuildingBlock.PermissionManagement.Dtos;
using MyBuildingBlock.PermissionManagement.Services;
using MyBuildingBlock.Web;
using StarterKit_Test.Data;
using System.Reflection;

namespace StarterKit_Test.Services.PermissionsManagementInfo
{
    [Route($"{BaseConstants.BASE_URL}/Permissions/v1")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [AdminOnly(enumActionType.All)]
    [AdminBranchOnly(enumActionType.All)]
    [BasePermission(true)]
    public class PermissionsController : SecureControllerBase
    {
        private readonly IPermissionDiscoveryService _discoveryService;


        public PermissionsController(ApplicationDbContext context, ILogger<PermissionsController> logger, IMapper mapper, IServiceProvider service, ILookupMapper lookupMapper
            , IPermissionDiscoveryService discoveryService)
          : base(context, logger, mapper, service.GetRequiredService<IMediator>(), service)
        {
            _discoveryService = discoveryService;
        }
        [HttpGet]
        public IActionResult GetAllPermissions()
        {
            var permissions = _discoveryService.GetAllPermissions(Assembly.GetExecutingAssembly());
            return Ok(permissions);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveAsync([FromBody] SavePermissionDto request)
        {
            try
            {

                var userContext = GetUserContext();
                if (request.roleId == 1)
                {
                    throw new CustomValidationException("لا يمكن التلاعب بصلاحية مدراء النظام");
                }
                var oldPerms = await _context.Set<UserRolePermissions>().Where(f => f.RoleLevelId == request.roleId).ToListAsync();
                var permHistory = new List<UserRolePermissionHistory>();

                if (oldPerms.Any())
                {
                    foreach (var item in oldPerms)
                    {

                        if (!request.permissions.Contains(item.PermissionPath))
                        {
                            permHistory.Add(new UserRolePermissionHistory()
                            {
                                PermissionPath = item.PermissionPath,
                                RoleLevelId = request.roleId,
                                PermissionAddDate = item.AddTime,
                                AddTime = DateTime.UtcNow,
                                Value = item.Value,
                                UserID = userContext.UserId,
                                PermissionAddUserId = item.UserID,
                                SessionID = userContext.SessionId,
                                BranchID = 1
                            });
                            _context.Remove(item);
                        }

                    }

                }
                if (permHistory.Any())
                    await _context.AddRangeAsync(permHistory);

                foreach (var item in request.permissions.Distinct().ToList())
                {
                    if (oldPerms.Any())
                    {
                        if (!oldPerms.Where(f => f.PermissionPath == item).Any())
                            _context.Add(new UserRolePermissions()
                            {
                                PermissionPath = item,
                                RoleLevelId = request.roleId,
                                AddTime = DateTime.UtcNow
                            ,
                                UserID = userContext.UserId,
                                BranchID = userContext.BranchId,
                                SessionID = userContext.SessionId
                            });
                    }
                    else
                    {
                        _context.Add(new UserRolePermissions()
                        {
                            PermissionPath = item,
                            RoleLevelId = request.roleId,
                            AddTime = DateTime.UtcNow
                                              ,
                            UserID = userContext.UserId,
                            BranchID = userContext.BranchId,
                            SessionID = userContext.SessionId
                        });
                    }


                }
                await _context.SaveChangesAsync();


                return Ok();
            }
            catch (CustomException ex)
            {

                throw ex;
            }
        }

        [HttpGet("role/{id}")]
        [ActionPermission]
        public async Task<IActionResult> GetRolePermissionsAsync(long id)
        {
            var perms = await _context.Set<UserRolePermissions>().AsNoTracking().Where(f => f.RoleLevelId == id).Select(f => f.PermissionPath).ToListAsync();
            return Ok(perms);

        }
        [HttpGet("role")]
        public async Task<IActionResult> GetRolePermissionsAsync()
        {
            var userContext = GetUserContext();
            var perms = await _context.Set<UserRolePermissions>().AsNoTracking().Where(f => f.RoleLevelId == userContext.RoleId).Select(f => f.PermissionPath).ToListAsync();
            return Ok(perms);

        }

    }

}
