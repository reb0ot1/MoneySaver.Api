using MoneySaver.Api.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Services.Models
{
    public class BudgetModel
    {
        public IList<BudgetItemModel> BudgetItems { get; set; }

        public double LimitAmount { get; set; }

        public double TotalSpentAmmount { get; set; }

        public double TotalLeftAmount { get; set; }
    }
}
