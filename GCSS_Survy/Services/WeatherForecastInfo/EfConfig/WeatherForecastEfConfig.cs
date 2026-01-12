using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuildingBlock.Ef.EfConfig;
using GCSS_Survy.Services.WeatherForecastInfo.Models;

namespace GCSS_Survy.Services.WeatherForecastInfo.EfConfig
{
    public class WeatherForecastEfConfig : CustomBaseEfConfig<WeatherForecast>
    {
        public override void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            base.Configure(builder);
        }
    }
}
