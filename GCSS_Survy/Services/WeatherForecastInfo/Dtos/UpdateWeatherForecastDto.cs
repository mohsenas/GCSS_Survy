using MyBuildingBlock.Abstracts;

namespace GCSS_Survy.Services.WeatherForecastInfo.Dtos
{
    public class UpdateWeatherForecastDto : BaseUpdateRequestDto<int>
    {
        
        public DateOnly? Date { get; set; }

        public int? TemperatureC { get; set; }

        public string? Summary { get; set; }
        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
