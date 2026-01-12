using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuildingBlock.Ef.EfConfig;
using StarterKit_Test.Services.Emp_SimpleInfo.Models;

namespace StarterKit_Test.Services.Emp_SimpleInfo.EFConfig
{
    public sealed class EmpEfConfig : CustomBaseEfConfig<Emp_Simples>
    {
        public override void Configure(EntityTypeBuilder<Emp_Simples> builder)
        {
            base.Configure(builder);
        }
    }
}

