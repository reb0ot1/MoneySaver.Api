using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MoneySaver.Api.Data
{
    public class BudgetItem
    {
        [Column("Id")]
        public int BudgetItemId { get; set; }

        public int BudgetId { get; set; }

        public Budget Budget { get; set; }

        public int TransactionCategoryId { get; set; }

        public TransactionCategory TransactionCategory { get; set; }

        public double LimitAmount { get; set; }

        public double SpentAmount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }
    }
}
