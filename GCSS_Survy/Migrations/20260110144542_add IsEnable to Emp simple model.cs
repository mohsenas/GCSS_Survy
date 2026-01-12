using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GCSS_Survy.Migrations
{
    /// <inheritdoc />
    public partial class addIsEnabletoEmpsimplemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Emp_Simples",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Emp_Simples");
        }
    }
}
