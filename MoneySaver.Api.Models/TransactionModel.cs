using System;

namespace MoneySaver.Api.Models
{
    public class TransactionModel
    {
        public string Id { get; set; }

        public DateTime TransactionDate { get; set; }

        public int TransactionCategoryId { get; set; }

        public double Amount { get; set; }

        public string AdditionalNote { get; set; }
    }
}
