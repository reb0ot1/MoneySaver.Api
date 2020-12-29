using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Add_ParentId_In_TransactionCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "TransactionCategoryId1",
                table: "TransactionCategories");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "TransactionCategories",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "DeletedOnUtc", "IsDeleted", "ModifyOn", "TransactionCategoryId", "TransactionDate", "UserId" },
                values: new object[] { new Guid("d54aa3e2-f919-46ca-bcaa-5459d8206b16"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 12, 13, 6, 27, 46, 623, DateTimeKind.Utc).AddTicks(260), null, false, new DateTime(2020, 12, 13, 6, 27, 46, 623, DateTimeKind.Utc).AddTicks(1261), 1, new DateTime(2020, 12, 13, 6, 27, 46, 623, DateTimeKind.Utc).AddTicks(2367), 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d54aa3e2-f919-46ca-bcaa-5459d8206b16"));

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "TransactionCategories");

            migrationBuilder.AddColumn<int>(
                name: "TransactionCategoryId1",
                table: "TransactionCategories",
                type: "int",
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
    }
}
