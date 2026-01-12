using MapsterMapper;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Data;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Localization;
using GCSS_Survy.Data;
using GCSS_Survy.Services.WeatherForecastInfo.Dtos;
using GCSS_Survy.Services.WeatherForecastInfo.Models;

namespace GCSS_Survy.Services.WeatherForecastInfo.Services
{
    public class WeatherForecastService : RESTfulServiceBase<WeatherForecast, AddWeatherForecastDto, UpdateWeatherForecastDto, WeatherForecastResult, int>
    {
        public WeatherForecastService(ApplicationDbContext context, IMapper mapper, ILookupMapper lookupMapper, ILogger<WeatherForecastService> logger, ILocalizationService localizationService) : base(context, mapper, lookupMapper, logger, localizationService)
        {

        }
     
    }
}
