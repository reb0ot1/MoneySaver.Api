using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneySaver.Api.Data
{
    public class BudgetItem : IUser, IDeletable
    {
        [Column("Id")]
        public int Id { get; set; }

        public int TransactionCategoryId { get; set; }

        public virtual TransactionCategory TransactionCategory { get; set; }

        public int BudgetId { get; set; }

        public virtual Budget Budget {get; set;}

        public double LimitAmount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }

        public string UserId { get; set; }
    }
}
