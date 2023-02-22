using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneySaver.Api.Data.Migrations
{
    public partial class AppConfig_Table_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.CreateTable(
                name: "AppConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BudgetType = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfigurations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppConfigurations");

            migrationBuilder.InsertData(
                table: "TransactionCategories",
                columns: new[] { "Id", "DeletedOnUtc", "IsDeleted", "Name", "ParentId", "UserId" },
                values: new object[] { 1, null, false, "Food", null, null });

            migrationBuilder.InsertData(
                table: "TransactionCategories",
                columns: new[] { "Id", "DeletedOnUtc", "IsDeleted", "Name", "ParentId", "UserId" },
                values: new object[] { 2, null, false, "Sport", null, null });
        }
    }
}
