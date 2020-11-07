namespace MoneySaver.Api.Data
{
    public class BudgetTransactionCategory
    {
        public int BudgetId { get; set; }

        public Budget Budget { get; set; }

        public int TransactionCategoryId { get; set; }
         
        public TransactionCategory TransactionCategory { get; set; }
    }
}