using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Attributes;
using MyBuildingBlock.EfConfig;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCSS_Survy.Services.CompanyInfo.Models
{
    [Index(nameof(CompanyName),IsUnique =true)]
    //[Table("tbl")]
    [Lookup(nameof(CompanyName),nameof(Address))]
    public record Companies : ParentEntity<long>
    {
        [Search(1)]
        public string CompanyName { get; set; }
        [Search(2)]
        public string? Address { get; set; }
        public int ManagerId { get; set; }
        public bool IsEnable { get; set; }
        public bool  IsDeleted { get; set; }

    }
}
