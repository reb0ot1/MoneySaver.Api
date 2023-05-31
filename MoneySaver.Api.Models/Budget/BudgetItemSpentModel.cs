namespace MoneySaver.Api.Models.Budget
{
    public class BudgetItemSpentModel : BudgetItemModel
    {
        public double SpentAmount { get; set; }

        //TODO: Remove this. It should be in the UI
        public int Progress { get; private set; }

        public void CalculateProgress()
        {
            Progress = 100 - (int)(SpentAmount / LimitAmount * 100);
        }
    }
}
