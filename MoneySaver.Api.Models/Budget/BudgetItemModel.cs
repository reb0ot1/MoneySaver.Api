namespace MoneySaver.Api.Models.Budget
{
    public class BudgetItemModel
    {
        public int Id { get; set; }

        public int TransactionCategoryId { get; set; }

        public string TransactionCategoryName { get; set; }

        public double LimitAmount { get; set; }

        public double SpentAmount { get; set; }

        public int Progress { get; private set; }

        public void CalculateProgress()
        {
            Progress = 100 - (int)(SpentAmount / LimitAmount * 100);
        }
    }
}
