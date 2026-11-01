using Microsoft.EntityFrameworkCore;
using System;

namespace MoneySaver.Api.Data
{
    public class MoneySaverApiContext : DbContext
    {
        public MoneySaverApiContext(DbContextOptions<MoneySaverApiContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions{ get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }
        public DbSet<AppConfiguration> AppConfigurations { get; set; }

        // Npgsql only accepts DateTime values with Kind=Utc for timestamptz columns, while the services build filter dates with Kind=Unspecified.
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    //seed categories
        //    //modelBuilder.Entity<TransactionCategory>().HasData(new TransactionCategory { TransactionCategoryId = 1, Name = "Food" });
        //    //modelBuilder.Entity<TransactionCategory>().HasData(new TransactionCategory { TransactionCategoryId = 2, Name = "Sport" });

        //    //modelBuilder.Entity<Transaction>().HasData(new Transaction
        //    //{
        //    //   Id = Guid.NewGuid(),
        //    //   Amount = 3.40,
        //    //   AdditionalNote = "Тест бележка",
        //    //   CreateOn = DateTime.UtcNow,
        //    //   ModifyOn = DateTime.UtcNow,
        //    //   TransactionDate = DateTime.UtcNow,
        //    //   TransactionCategoryId = 1,
        //    //   UserId = 1,
        //    //   IsDeleted = false
        //    //});
        //}
    }
}
