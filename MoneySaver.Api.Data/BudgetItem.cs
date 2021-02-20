using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MoneySaver.Api.Data
{
    public class BudgetItem : IUser
    {
        [Column("Id")]
        public int Id { get; set; }

        public int TransactionCategoryId { get; set; }

        public double LimitAmount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }

        public string UserId { get; set; }
    }
}
