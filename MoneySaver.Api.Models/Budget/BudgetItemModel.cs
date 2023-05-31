namespace MoneySaver.Api.Models.Budget
{
    public class BudgetItemModel
    {
        public int Id { get; set; }

        public int TransactionCategoryId { get; set; }

        public string TransactionCategoryName { get; set; }

        public double LimitAmount { get; set; }
    }
}
