using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneySaver.Api.Data
{
    public class AppConfiguration : IUser, IDeletable
    {
        [Column("Id")]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;

        public BudgetType BudgetType { get; set; } = BudgetType.Monthly;

        public CurrencyType Currency { get; set; } = CurrencyType.BGN;

        public bool IsDeleted { get; set; }

        public string UserId { get; set; }
    }
}
