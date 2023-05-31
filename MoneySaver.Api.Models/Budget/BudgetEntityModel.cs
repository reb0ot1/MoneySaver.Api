using System;
using System.Collections.Generic;

namespace MoneySaver.Api.Models.Budget
{
    public class BudgetEntityModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public BudgetType BudgetType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsInUse { get; set; }

        public IEnumerable<BudgetItemModel> BudgetItems { get; set; }
    }
}
