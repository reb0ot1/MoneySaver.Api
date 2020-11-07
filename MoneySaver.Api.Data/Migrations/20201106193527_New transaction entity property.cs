using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Newtransactionentityproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("3fdbbfd3-a9bf-4de7-90e2-d9db8a823ec3"));

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TransactionCategories",
                newName: "TransactionCategoryId");

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "Transactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("39c259b0-a9a9-44ce-b886-b52522360b82"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 6, 19, 35, 27, 470, DateTimeKind.Utc).AddTicks(5842), new DateTime(2020, 11, 6, 19, 35, 27, 470, DateTimeKind.Utc).AddTicks(6317), 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("39c259b0-a9a9-44ce-b886-b52522360b82"));

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionCategoryId",
                table: "TransactionCategories",
                newName: "Id");

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "ModifyOn", "TransactionCategoryId", "UserId" },
                values: new object[] { new Guid("3fdbbfd3-a9bf-4de7-90e2-d9db8a823ec3"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 6, 18, 56, 50, 221, DateTimeKind.Utc).AddTicks(8815), new DateTime(2020, 11, 6, 18, 56, 50, 221, DateTimeKind.Utc).AddTicks(9597), 1, 1 });
        }
    }
}
