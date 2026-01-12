using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Abstracts;
using MyBuildingBlock.Attributes;
using MyBuildingBlock.Exceptions;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Lookup;
using MyBuildingBlock.PermissionManagement.Attributes;
using MyBuildingBlock.PermissionManagement.Dtos;
using GCSS_Survy.Data;
using GCSS_Survy.Services.Emp_SimpleInfo.Dtos;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;
using GCSS_Survy.Services.Emp_SimpleInfo.Services;



namespace GCSS_Survy.Services.Emp_SimpleInfo.Controllers
{
    [Route($"{BaseConstants.BASE_URL}/[Controller]/v1")]
    [AdminBranchOnly]
    [BasePermission("الموظفين", PermissionConstants.TREE_NODE_ICON, PermissionConstants.ROOT_CODE, "Employee")]
    /// <summary>
    /// Employee controller using simplified ControllerBase pattern with type alias.
    /// The type alias 'EmployeeControllerBase' eliminates the need to repeat generic parameters,
    /// making the class declaration much cleaner while maintaining full type safety.
    /// </summary>
    public  class EmployeeController : ControllerBase<Emp_SimpleService,AddEmpDto,UpdateEmpDto,EmpResult,
    int,
   ApplicationDbContext>
    {
        public EmployeeController(
            ApplicationDbContext context,
            ILogger<EmployeeController> logger,
            IMapper mapper,
            IServiceProvider service,
            ILookupMapper lookupMapper)
            : base(context, logger, mapper, service, lookupMapper)
        {
        }

        [HttpPut("ActivateEmp/{id}&{isActive}")]
        [ActionPermission("تفعيل/إلغاء تفعيل موظف", PermissionConstants.BLOCK_ICON)]
        public async Task<IActionResult> ActivateEmp(int id, bool isActive)
        {
            var emp = await _context.Set<Emp_Simples>().Where(f => f.ID == id).FirstOrDefaultAsync();
            if (emp is null)
                throw new CustomNotFoundException("Employee not found");

            emp.IsEnabled = isActive;
            await _context.SaveChangesAsync();
            var result = _mapper.Map<EmpResult>(emp);
            return Ok(result);
        }
    }
}

