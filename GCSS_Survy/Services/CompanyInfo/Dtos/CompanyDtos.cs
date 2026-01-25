using MyBuildingBlock.Abstracts;
using MyBuildingBlock.Attributes;

namespace GCSS_Survy.Services.CompanyInfo.Dtos
{
    public class AddCompanyDto:BaseAddRequestDto<long>
    {
        public string CompanyName { get; set; }
        public string? Address { get; set; }
        public int ManagerId { get; set; }
        public bool IsEnable { get; set; }
    }

    public class UpdateCompanyDto : BaseUpdateRequestDto<long>
    {
        public string? Address { get; set; }
        public int? ManagerId { get; set; }
        public bool? IsEnable { get; set; }
        public string? Notes { get; set; }
    }
    public record CompanyResult : BaseResult<long>
    {
        public string CompanyName { get; set; }
        public string? Address { get; set; }
        public int? ManagerId { get; set; }
        public bool? IsEnable { get; set; }
        public bool? IsDeleted { get; set; }
    }


}
