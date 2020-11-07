using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Services.Models
{
    public class BudgetItemModel
    {
        public int Id { get; set; }

        public int BudgetId { get; set; }

        public int TransactionCategoryId { get; set; }

        public double LimitAmount { get; set; }

        public double SpentAmount { get; set; }
    }
}
