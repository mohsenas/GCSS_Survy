using GCSS_Survy.Data;
using GCSS_Survy.Services.CompanyInfo.Dtos;
using GCSS_Survy.Services.CompanyInfo.Models;
using GCSS_Survy.Services.Emp_SimpleInfo.Dtos;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Exceptions;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Localization;
using MyBuildingBlock.Web;
using System.Runtime.CompilerServices;

namespace GCSS_Survy.Services.CompanyInfo.Services
{
    public class CompanyService : RESTfulServiceBase<Companies, AddCompanyDto, UpdateCompanyDto, CompanyResult, long>
    {
        public CompanyService(ApplicationDbContext context, IMapper mapper, ILookupMapper lookupMapper, ILogger<CompanyService> logger, ILocalizationService localizationService) : base(context, mapper, lookupMapper, logger, localizationService)
        {

        }


        public override async Task<ReturnResult<CompanyResult>> AddAsync(AddCompanyDto request, UserContext userContext)
        {
            if (request.CompanyName == null)
                throw new CustomValidationException("يجب ادخال اسم الشركة");
         var nameExists=await   _context.Set<Companies>().Where(f=>f.CompanyName== request.CompanyName).AnyAsync();
            if (nameExists)
                throw new CustomValidationException("اسم الشركة موجود مسبقا");
             
            return await base.AddAsync(request, userContext);
        }

    }
  
}
