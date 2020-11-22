using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class added_idDeleted_and_DeletedOn_to_TransactioCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("1621823d-be10-4b50-a112-dedf871eab5e"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOnUtc",
                table: "TransactionCategories",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TransactionCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TransactionCategoryId1",
                table: "TransactionCategories",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "DeletedOnUtc", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("95005f25-0d96-4406-bb11-fd6df59edddd"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 22, 12, 36, 31, 861, DateTimeKind.Utc).AddTicks(6497), null, false, new DateTime(2020, 11, 22, 12, 36, 31, 861, DateTimeKind.Utc).AddTicks(7121), 1, new DateTime(2020, 11, 22, 12, 36, 31, 861, DateTimeKind.Utc).AddTicks(7641), 1 });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategories_TransactionCategoryId1",
                table: "TransactionCategories",
                column: "TransactionCategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionCategories_TransactionCategories_TransactionCategoryId1",
                table: "TransactionCategories",
                column: "TransactionCategoryId1",
                principalTable: "TransactionCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionCategories_TransactionCategories_TransactionCategoryId1",
                table: "TransactionCategories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionCategories_TransactionCategoryId1",
                table: "TransactionCategories");

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("95005f25-0d96-4406-bb11-fd6df59edddd"));

            migrationBuilder.DropColumn(
                name: "DeletedOnUtc",
                table: "TransactionCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TransactionCategories");

            migrationBuilder.DropColumn(
                name: "TransactionCategoryId1",
                table: "TransactionCategories");

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "DeletedOnUtc", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("1621823d-be10-4b50-a112-dedf871eab5e"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 22, 10, 14, 27, 287, DateTimeKind.Utc).AddTicks(9141), null, false, new DateTime(2020, 11, 22, 10, 14, 27, 287, DateTimeKind.Utc).AddTicks(9696), 1, new DateTime(2020, 11, 22, 10, 14, 27, 288, DateTimeKind.Utc).AddTicks(190), 1 });
        }
    }
}
