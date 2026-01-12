using MyBuildingBlock.Abstracts;
using MyBuildingBlock.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Dtos
{
    public class AddEmpDto : BaseAddRequestDto<int>
    {
        public string EmpName { get; set; }

      
        public string Job { get; set; }

        public DateTime JobDate { get; set; }
        public string? Email { get; set; }

        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
