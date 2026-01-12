using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuildingBlock.Ef.EfConfig;
using StarterKit_Test.Services.WeatherForecastInfo.Models;

namespace StarterKit_Test.Services.WeatherForecastInfo.EfConfig
{
    public class WeatherForecastEfConfig : CustomBaseEfConfig<WeatherForecast>
    {
        public override void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            base.Configure(builder);
        }
    }
}
