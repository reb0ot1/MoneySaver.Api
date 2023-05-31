using MoneySaver.Api.Models;
using System;
using System.Collections.Generic;

namespace MoneySaver.Api.Data
{
    public class Budget : IUser, IDeletable
    {
        public Budget()
        {
            var dateTimeNow = DateTime.UtcNow;
            this.CreatedOn = dateTimeNow;
            this.ModifiedOn = dateTimeNow;
            this.BudgetItems= new List<BudgetItem>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public BudgetType BudgetType { get; set; }

        public bool IsInUse { get; set; }

        public virtual IEnumerable<BudgetItem> BudgetItems { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }

        public string UserId { get; set; }
    }
}