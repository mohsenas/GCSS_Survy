using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using MyBuildingBlock.Abstracts;
using MyBuildingBlock.Attributes;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.PermissionManagement.Attributes;
using StarterKit_Test.Data;
using StarterKit_Test.Services.WeatherForecastInfo.Dtos;
using StarterKit_Test.Services.WeatherForecastInfo.Services;

namespace StarterKit_Test.Services.WeatherForecastInfo.Controllers
{
   
    [Route($"{BaseConstants.BASE_URL}/[Controller]/v1")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [AdminOnly]
    [AdminBranchOnly]
    [BasePermission(true)]
    public class WeatherForecastController : ControllerBase<WeatherForecastService, AddWeatherForecastDto, UpdateWeatherForecastDto, WeatherForecastResult, int, ApplicationDbContext>
    {
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceProvider sp, ILookupMapper lookupMapper, ApplicationDbContext context, IMapper mapper)
            : base(context: context, logger, mapper, sp, lookupMapper)
        {
        }
    }
    }
