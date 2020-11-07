using System;
using System.Collections;
using System.Collections.Generic;

namespace MoneySaver.Api.Data
{
    public class Budget
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public BudgetType Type { get; set; }

        public IList<BudgetItem> BudgetItems { get; set; }

        public double LimitAmount { get; set; }
    }
}