using System;

namespace MoneySaver.Api.Data
{
    public class Budget : IUser, IDeletable
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public BudgetType Type { get; set; }

        public double LimitAmount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }

        public string UserId { get; set; }
    }
}