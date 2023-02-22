using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneySaver.Api.Data.Migrations
{
    public partial class Change_Budget_Entity_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitAmount",
                table: "Budgets");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Budgets",
                newName: "BudgetType");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Budgets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCurenttlyInUse",
                table: "Budgets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Budgets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Budgets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "BudgetItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "EndDate",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "IsCurenttlyInUse",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "BudgetItems");

            migrationBuilder.RenameColumn(
                name: "BudgetType",
                table: "Budgets",
                newName: "Type");

            migrationBuilder.AddColumn<double>(
                name: "LimitAmount",
                table: "Budgets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
