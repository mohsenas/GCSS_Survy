using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuildingBlock.Ef.EfConfig;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;

namespace GCSS_Survy.Services.Emp_SimpleInfo.EFConfig
{
    public sealed class EmpEfConfig : CustomBaseEfConfig<Emp_Simples>
    {
        public override void Configure(EntityTypeBuilder<Emp_Simples> builder)
        {
            base.Configure(builder);
        }
    }
}

