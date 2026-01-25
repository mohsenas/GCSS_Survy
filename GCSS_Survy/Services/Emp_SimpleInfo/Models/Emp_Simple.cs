using MyBuildingBlock.Attributes;
using MyBuildingBlock.Ef;
using MyBuildingBlock.EfConfig;
using System.ComponentModel.DataAnnotations;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Models
{
    [DocCode(nameof(Emp_Simples))]
    [Lookup(nameof(EmpName), nameof(Job))]

    public record Emp_Simples : ParentEntity<int>,IHasCustomFields
    {
        [Search(1)]
        [MaxLength(100)]
        public string EmpName { get; set; }

        [Search(2)]
        [MaxLength(100)]
        public string Job { get; set; }

        public DateTime JobDate { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public Dictionary<string, string> CustomFields { get; set; }
    }
}
