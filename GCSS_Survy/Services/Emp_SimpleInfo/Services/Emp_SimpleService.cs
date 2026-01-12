using MapsterMapper;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Data;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Localization;
using MyBuildingBlock.Lookup;
using GCSS_Survy.Data;
using GCSS_Survy.Services.Emp_SimpleInfo.Dtos;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Services
{
    public class Emp_SimpleService : RESTfulServiceBase<Emp_Simples, AddEmpDto, UpdateEmpDto, EmpResult, int>
    {
        public Emp_SimpleService(ApplicationDbContext context, IMapper mapper, ILookupMapper lookupMapper, ILogger<Emp_SimpleService> logger, ILocalizationService localizationService) : base(context, mapper, lookupMapper, logger, localizationService)
        {

        }

    }
   
}

