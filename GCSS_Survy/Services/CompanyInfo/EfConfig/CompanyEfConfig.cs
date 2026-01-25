using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuildingBlock.Ef.EfConfig;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;
using GCSS_Survy.Services.CompanyInfo.Models;

namespace GCSS_Survy.Services.CompanyInfo.EFConfig
{
    public sealed class CompanyEfConfig : CustomBaseEfConfig<Companies>
    {
        public override void Configure(EntityTypeBuilder<Companies> builder)
        {
            base.Configure(builder);
            builder.Property(f => f.CompanyName).IsRequired().HasMaxLength(255);
        }
    }
}

