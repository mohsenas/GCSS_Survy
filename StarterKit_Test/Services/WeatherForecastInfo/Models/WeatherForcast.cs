using MyBuildingBlock.Attributes;
using MyBuildingBlock.Ef;
using MyBuildingBlock.EfConfig;
using System.ComponentModel.DataAnnotations;

namespace StarterKit_Test.Services.WeatherForecastInfo.Models
{
    [DocCode(nameof(WeatherForecast))]
    [Lookup(nameof(Date), nameof(TemperatureC))]

    public record WeatherForecast:ParentEntity<int>,IHasCustomFields
    {
        [Search(1)]
        public DateOnly Date { get; set; }

        [Search(2)]
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [MaxLength(100)]
        public string? Summary { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
    }
}
