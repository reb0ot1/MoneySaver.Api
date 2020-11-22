using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class added_isDeleted_and_deletedOn_properties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("a77645ba-1128-4ef6-8f47-122d0a255ed4"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOnUtc",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOnUtc",
                table: "Budgets",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Budgets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOnUtc",
                table: "BudgetItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BudgetItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "DeletedOnUtc", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("1621823d-be10-4b50-a112-dedf871eab5e"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 22, 10, 14, 27, 287, DateTimeKind.Utc).AddTicks(9141), null, false, new DateTime(2020, 11, 22, 10, 14, 27, 287, DateTimeKind.Utc).AddTicks(9696), 1, new DateTime(2020, 11, 22, 10, 14, 27, 288, DateTimeKind.Utc).AddTicks(190), 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("1621823d-be10-4b50-a112-dedf871eab5e"));

            migrationBuilder.DropColumn(
                name: "DeletedOnUtc",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeletedOnUtc",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "DeletedOnUtc",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BudgetItems");

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("a77645ba-1128-4ef6-8f47-122d0a255ed4"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 15, 14, 12, 36, 82, DateTimeKind.Utc).AddTicks(4736), false, new DateTime(2020, 11, 15, 14, 12, 36, 82, DateTimeKind.Utc).AddTicks(5071), 1, new DateTime(2020, 11, 15, 14, 12, 36, 82, DateTimeKind.Utc).AddTicks(5370), 1 });
        }
    }
}
