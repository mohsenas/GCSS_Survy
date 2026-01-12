using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Ef;
using MyBuildingBlock.EfConfig;
using MyBuildingBlock.Models;
using MyBuildingBlock.ModuleParameterInfo.Models;
using MyBuildingBlock.Security.Data;
using MyBuildingBlock.Security.EfConfig;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Xml;
using MyBuildingBlock.Data;
using MyBuildingBlock.Security.Models;
using DigitalArchiving_API.Contracts.TransactionInfo.Models;
using MyBuildingBlock.NotificationManagement.Abstracts;
using MyBuildingBlock.ChangeTracking.Data;
using Microsoft.EntityFrameworkCore.Design;

namespace GCSS_Survy.Data
{
    public partial class ApplicationDbContext : DbContextBase
    //, ISecurityDbContext, IChangeHistoryDbContext
    {
        //public override DbSet<Users> Users { get; set; }
        //public override DbSet<Branches> Branches { get; set; }
        //public override DbSet<UserRolePermissions> UserRolePermissions { get; set; }
        //public override DbSet<UserRolePermissionHistory> UserRolePermissionHistory { get; set; }
        //public override DbSet<NotificationBase> Notifications { get; set; }
        //public override DbSet<WarningIgnores> WarningIgnores { get; set; }
        //public override DbSet<Ticket> Tickets { get; set; }
        //public override DbSet<TicketAttachment> TicketAttachments { get; set; }
        //public override DbSet<TicketHistory> TicketHistories { get; set; }
        //public override DbSet<TicketComment> TicketComments { get; set; }
        //public override DbSet<TicketMember> TicketMembers { get; set; }
        //public override DbSet<TicketMemberCategory> TicketMemberCategories { get; set; }
        //public override DbSet<LoginAttempt> LoginAttempts { get; set; }
        //public override DbSet<SecurityEvent> SecurityEvents { get; set; }
        //public override DbSet<AccountLockout> AccountLockouts { get; set; }
        //public override DbSet<IpWhitelist> IpWhitelists { get; set; }
        //public override DbSet<IpBlacklist> IpBlacklists { get; set; }
        //public override DbSet<RefreshToken> RefreshTokens { get; set; }
        //public override DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        //public override DbSet<EntityChangeHistory> EntityChangeHistories { get; set; }

       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

      

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var bbEntityTypes = this.GetType().Assembly
            .GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                         && IsSubclassOfRawGeneric(typeof(ParentEntity<>), t));
            modelBuilder.Entity(typeof(ModuleParameters));

            foreach (var type in bbEntityTypes)
            {
                modelBuilder.Entity(type);
            }

            modelBuilder.HasAnnotation("Relational:Collation", "Arabic_CI_AS");

            new DbInitializer().Seed(modelBuilder);
        }
       
    }


}
