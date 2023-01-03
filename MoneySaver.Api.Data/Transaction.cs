using System;

namespace MoneySaver.Api.Data
{
    public class Transaction : IUser, IDeletable
    {
        public Guid Id { get; set; }

        public DateTime CreateOn { get; set; } = DateTime.UtcNow;

        public DateTime ModifyOn { get; set; } = DateTime.UtcNow;

        public DateTime TransactionDate { get; set; }

        public string UserId { get; set; }

        public int TransactionCategoryId { get; set; }

        public TransactionCategory TransactionCategory { get; set; }

        public double Amount { get; set; }

        public string AdditionalNote { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }
    }
}
