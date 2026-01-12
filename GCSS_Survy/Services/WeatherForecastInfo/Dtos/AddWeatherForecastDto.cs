using MyBuildingBlock.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace GCSS_Survy.Services.WeatherForecastInfo.Dtos
{
    public class AddWeatherForecastDto : BaseAddRequestDto<int>
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }
        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
