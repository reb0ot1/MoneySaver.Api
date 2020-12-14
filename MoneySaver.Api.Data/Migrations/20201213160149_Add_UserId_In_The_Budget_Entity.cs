using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Add_UserId_In_The_Budget_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d6f2349d-ad83-4463-8566-9cdb4805c47f"));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Transactions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Budgets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Budgets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Transactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "DeletedOnUtc", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("d6f2349d-ad83-4463-8566-9cdb4805c47f"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(1945), null, false, new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(3131), 1, new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(4062), 1 });
        }
    }
}
