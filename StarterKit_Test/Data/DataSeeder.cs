using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Models;
using System.Reflection.Metadata;
using System.Xml;

namespace StarterKit_Test.Data
{
    public class DataSeeder
    {
    }
    public class DbInitializer
    {
        public void Seed(ModelBuilder builder)
        {
            // Seed your data here

            builder.Entity<Branches>().HasData(
                new Branches() { ID = 1, BranchNumber = 1, BranchName = "Main", TheNumber = 1, BranchCode = "Main", BranchID = 1, UserID = 1 }
            );
            //builder.Entity<tblUsersGroup>().HasData(
            //    new tblUsersGroup() { ID = 1, GroupName = "مدراء النظام", TheNumber = 1 });

            //builder.Entity<tblRule>().HasData(
            //    new tblRule() { ID = 1, TheName = "مدراء النظام", isEnabled = true }
            //);

            builder.Entity<Users>().HasData(
                new Users()
                {
                    ID = 1,
                    TheNumber = 1,
                    Fname = "admin",
                    Lname = "",
                    UserName = "admin",
                    DefaultBranchID = 1,
                    EnableFA = false,
                    ExpireInterval = 100,
                    Has2FA = false,
                    IsEnabled = true,
                    MustChangePassword = true,
                    MustRegNew2FA = true,
                    BranchID = 1,
                    UserID = 1,
                    prvtK = "",
                    Notes = "",
                    RuleID = 1

                ,
                    Password = "64462E6496301F6266436456376266444939948A45C7086B2CC49AF869016EC663A4C47998A7386B5B56B125DA264239",
                    UserGroupID = 1
                }
                );
         
        }
    }

}
