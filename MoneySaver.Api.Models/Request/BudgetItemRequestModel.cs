namespace MoneySaver.Api.Models.Request
{
    public class BudgetItemRequestModel
    {
        public int TransactionCategoryId { get; set; }

        public double LimitAmount { get; set; }
    }
}
