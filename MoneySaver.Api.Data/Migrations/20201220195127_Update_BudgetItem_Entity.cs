using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Update_BudgetItem_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetItems_Budgets_BudgetId",
                table: "BudgetItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetItems_TransactionCategories_TransactionCategoryId",
                table: "BudgetItems");

            migrationBuilder.DropIndex(
                name: "IX_BudgetItems_BudgetId",
                table: "BudgetItems");

            migrationBuilder.DropIndex(
                name: "IX_BudgetItems_TransactionCategoryId",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "SpentAmount",
                table: "BudgetItems");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BudgetItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BudgetItems");

            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "BudgetItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SpentAmount",
                table: "BudgetItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetId",
                table: "BudgetItems",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_TransactionCategoryId",
                table: "BudgetItems",
                column: "TransactionCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetItems_Budgets_BudgetId",
                table: "BudgetItems",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetItems_TransactionCategories_TransactionCategoryId",
                table: "BudgetItems",
                column: "TransactionCategoryId",
                principalTable: "TransactionCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
