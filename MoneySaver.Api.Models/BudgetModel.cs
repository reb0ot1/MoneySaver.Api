using System.Collections.Generic;

namespace MoneySaver.Api.Models
{
    public class BudgetModel
    {
        public IList<BudgetItemModel> BudgetItems { get; set; }

        public double LimitAmount { get; set; }

        public double TotalSpentAmmount { get; set; }

        public double TotalLeftAmount { get; set; }
    }
}
