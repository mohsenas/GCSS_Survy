using MyBuildingBlock.Abstracts;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Dtos
{
    public sealed record EmpResult: BaseResult<int>
    {

        public string EmpName { get; set; }


        public string Job { get; set; }

        public DateTime JobDate { get; set; }
        public string? Email { get; set; }
        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
