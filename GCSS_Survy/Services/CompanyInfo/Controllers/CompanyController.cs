using GCSS_Survy.Data;
using GCSS_Survy.Services.CompanyInfo.Dtos;
using GCSS_Survy.Services.CompanyInfo.Services;
using GCSS_Survy.Services.Emp_SimpleInfo.Dtos;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;
using GCSS_Survy.Services.Emp_SimpleInfo.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Abstracts;
using MyBuildingBlock.Attributes;
using MyBuildingBlock.Exceptions;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.PermissionManagement.Attributes;
using MyBuildingBlock.PermissionManagement.Dtos;

namespace GCSS_Survy.Services.CompanyInfo.Controllers
{
    [Route($"{BaseConstants.BASE_URL}/[Controller]/v1")]
    [AdminBranchOnly]
    //[AdminOnly]
    [BasePermission("المنشات", PermissionConstants.TREE_NODE_ICON, PermissionConstants.ROOT_CODE, "Company")]
    /// <summary>
    /// Employee controller using simplified ControllerBase pattern with type alias.
    /// The type alias 'EmployeeControllerBase' eliminates the need to repeat generic parameters,
    /// making the class declaration much cleaner while maintaining full type safety.
    /// </summary>
    public class CompanyController : ControllerBase<CompanyService, AddCompanyDto, UpdateCompanyDto, CompanyResult, long,
ApplicationDbContext>
    {
        public CompanyController(
            ApplicationDbContext context,
            ILogger<CompanyController> logger,
            IMapper mapper,
            IServiceProvider service,
            ILookupMapper lookupMapper)
            : base(context, logger, mapper, service, lookupMapper)
        {
        }

        
    }

   
}
