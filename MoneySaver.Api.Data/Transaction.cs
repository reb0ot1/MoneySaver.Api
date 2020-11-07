using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Data
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public DateTime CreateOn { get; set; }

        public DateTime ModifyOn { get; set; }

        public DateTime TransactionDate { get; set; }

        public int UserId { get; set; }

        public int TransactionCategoryId { get; set; }

        public TransactionCategory TransactionCategory { get; set; }

        public double Amount { get; set; }

        public string AdditionalNote { get; set; }
    }
}
