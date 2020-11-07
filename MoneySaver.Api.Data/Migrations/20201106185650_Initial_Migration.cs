using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetTransactionCategory",
                columns: table => new
                {
                    BudgetId = table.Column<int>(nullable: false),
                    TransactionCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetTransactionCategory", x => new { x.BudgetId, x.TransactionCategoryId });
                    table.ForeignKey(
                        name: "FK_BudgetTransactionCategory_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetTransactionCategory_TransactionCategories_TransactionCategoryId",
                        column: x => x.TransactionCategoryId,
                        principalTable: "TransactionCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateOn = table.Column<DateTime>(nullable: false),
                    ModifyOn = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    TransactionCategoryId = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    AdditionalNote = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionCategories_TransactionCategoryId",
                        column: x => x.TransactionCategoryId,
                        principalTable: "TransactionCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TransactionCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Food" });

            migrationBuilder.InsertData(
                table: "TransactionCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Sport" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AdditionalNote", "Amount", "CreateOn", "ModifyOn", "TransactionCategoryId", "UserId" },
                values: new object[] { new Guid("3fdbbfd3-a9bf-4de7-90e2-d9db8a823ec3"), "Тест бележка", 3.3999999999999999, new DateTime(2020, 11, 6, 18, 56, 50, 221, DateTimeKind.Utc).AddTicks(8815), new DateTime(2020, 11, 6, 18, 56, 50, 221, DateTimeKind.Utc).AddTicks(9597), 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactionCategory_TransactionCategoryId",
                table: "BudgetTransactionCategory",
                column: "TransactionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionCategoryId",
                table: "Transactions",
                column: "TransactionCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetTransactionCategory");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "TransactionCategories");
        }
    }
}
