using MapsterMapper;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Data;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Localization;
using MyBuildingBlock.Lookup;
using StarterKit_Test.Data;
using StarterKit_Test.Services.Emp_SimpleInfo.Dtos;
using StarterKit_Test.Services.Emp_SimpleInfo.Models;

namespace StarterKit_Test.Services.Emp_SimpleInfo.Services
{
    public class Emp_SimpleService : RESTfulServiceBase<Emp_Simples, AddEmpDto, UpdateEmpDto, EmpResult, int>
    {
        public Emp_SimpleService(ApplicationDbContext context, IMapper mapper, ILookupMapper lookupMapper, ILogger<Emp_SimpleService> logger, ILocalizationService localizationService) : base(context, mapper, lookupMapper, logger, localizationService)
        {

        }

    }
   
}

