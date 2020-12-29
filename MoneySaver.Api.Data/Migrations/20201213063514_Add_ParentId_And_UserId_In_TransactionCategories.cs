using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Add_ParentId_And_UserId_In_TransactionCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d54aa3e2-f919-46ca-bcaa-5459d8206b16"));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TransactionCategories",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "DeletedOnUtc", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("d6f2349d-ad83-4463-8566-9cdb4805c47f"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(1945), null, false, new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(3131), 1, new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(4062), 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d6f2349d-ad83-4463-8566-9cdb4805c47f"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TransactionCategories");

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "DeletedOnUtc", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("d54aa3e2-f919-46ca-bcaa-5459d8206b16"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 12, 13, 6, 27, 46, 623, DateTimeKind.Utc).AddTicks(260), null, false, new DateTime(2020, 12, 13, 6, 27, 46, 623, DateTimeKind.Utc).AddTicks(1261), 1, new DateTime(2020, 12, 13, 6, 27, 46, 623, DateTimeKind.Utc).AddTicks(2367), 1 });
        }
    }
}
