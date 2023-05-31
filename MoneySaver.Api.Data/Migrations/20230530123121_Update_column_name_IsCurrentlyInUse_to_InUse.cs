using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Update_column_name_IsCurrentlyInUse_to_InUse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCurenttlyInUse",
                table: "Budgets",
                newName: "IsInUse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsInUse",
                table: "Budgets",
                newName: "IsCurenttlyInUse");
        }
    }
}
