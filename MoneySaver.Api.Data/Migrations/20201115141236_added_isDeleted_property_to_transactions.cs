using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class added_isDeleted_property_to_transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("6a0389d5-df64-4cd3-8135-2be59455231b"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Transactions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("a77645ba-1128-4ef6-8f47-122d0a255ed4"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 15, 14, 12, 36, 82, DateTimeKind.Utc).AddTicks(4736), false, new DateTime(2020, 11, 15, 14, 12, 36, 82, DateTimeKind.Utc).AddTicks(5071), 1, new DateTime(2020, 11, 15, 14, 12, 36, 82, DateTimeKind.Utc).AddTicks(5370), 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("a77645ba-1128-4ef6-8f47-122d0a255ed4"));

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Transactions");

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("6a0389d5-df64-4cd3-8135-2be59455231b"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 7, 15, 56, 18, 594, DateTimeKind.Utc).AddTicks(8176), new DateTime(2020, 11, 7, 15, 56, 18, 594, DateTimeKind.Utc).AddTicks(8680), 1, new DateTime(2020, 11, 7, 15, 56, 18, 594, DateTimeKind.Utc).AddTicks(9132), 1 });
        }
    }
}
