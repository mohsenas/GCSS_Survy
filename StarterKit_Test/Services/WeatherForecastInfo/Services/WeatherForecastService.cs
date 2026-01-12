using MapsterMapper;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Data;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Localization;
using StarterKit_Test.Data;
using StarterKit_Test.Services.WeatherForecastInfo.Dtos;
using StarterKit_Test.Services.WeatherForecastInfo.Models;

namespace StarterKit_Test.Services.WeatherForecastInfo.Services
{
    public class WeatherForecastService : RESTfulServiceBase<WeatherForecast, AddWeatherForecastDto, UpdateWeatherForecastDto, WeatherForecastResult, int>
    {
        public WeatherForecastService(ApplicationDbContext context, IMapper mapper, ILookupMapper lookupMapper, ILogger<WeatherForecastService> logger, ILocalizationService localizationService) : base(context, mapper, lookupMapper, logger, localizationService)
        {

        }
     
    }
}
