using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.ChangeTracking.Data;
using MyBuildingBlock.ChangeTracking.Models;

namespace GCSS_Survy.Data
{
    public class LogDbContext : DbContext, IChangeHistoryDbContext
    {
       
        public  DbSet<EntityChangeHistory> EntityChangeHistories { get; set; }


        public LogDbContext(DbContextOptions<LogDbContext> options)
          : base(options)
        {
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
        }

    }


}
