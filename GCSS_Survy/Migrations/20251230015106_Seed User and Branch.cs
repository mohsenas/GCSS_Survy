using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GCSS_Survy.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserandBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "ID", "AddTime", "Address", "BranchCode", "BranchID", "BranchName", "BranchNumber", "Description", "Fax", "IsEnabled", "LastModifiedUserId", "LastModifyTime", "Notes", "Phone", "RV", "SessionID", "TheNumber", "UserID" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Main", 1, "Main", 1, null, null, true, null, null, null, null, null, 0L, 1, 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "AddTime", "BranchID", "ChangePasswordRequiredBefore", "DefaultBranchID", "EnableFA", "ExpireInterval", "FirstLogin", "Fname", "Has2FA", "IsEnabled", "IsExternalSys_user", "LastChange", "LastModifiedUserId", "LastModifyTime", "Lname", "MustChangePassword", "MustRegNew2FA", "Notes", "Password", "RV", "RuleID", "SessionID", "TheNumber", "UserGroupID", "UserID", "UserImage", "UserName", "prvtK" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1, false, 100, null, "", false, true, null, null, null, null, "", true, true, "", "64462E6496301F6266436456376266444939948A45C7086B2CC49AF869016EC663A4C47998A7386B5B56B125DA264239", null, 1L, 0L, 1, 1, 1, null, "admin", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID",
                keyValue: 1);
        }
    }
}
