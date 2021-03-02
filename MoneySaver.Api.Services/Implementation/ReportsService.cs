using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class ReportsService : IReportsService
    {
        private readonly ILogger<ReportsService> logger;
        private readonly IRepository<Transaction> transactionRepository;
        private readonly IRepository<TransactionCategory> transactionCategoryRepository;

        public ReportsService(
            ILogger<ReportsService> logger,
            IRepository<Transaction> transactionRepository,
            IRepository<TransactionCategory> transactionCategoryRepository
            )
        {
            this.logger = logger;
            this.transactionRepository = transactionRepository;
            this.transactionCategoryRepository = transactionCategoryRepository;
        }

        public async Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter)
        {
            var dataItems = new List<DataItem>();
            var query = from transactionItem in this.transactionRepository.GetAll()
                        .Where(e => !e.IsDeleted && e.TransactionDate >= filter.From && e.TransactionDate <= filter.To) 
                        join transCategory in this.transactionCategoryRepository.GetAll()
                            on transactionItem.TransactionCategoryId equals transCategory.TransactionCategoryId
                        select new { transactionItem, transCategory };

            var result = await query.ToListAsync();
            var totalSum = result.Sum(tr => tr.transactionItem.Amount);
            var groupTransactionTypes = result.GroupBy(gr => gr.transactionItem.TransactionCategoryId);
            foreach (var group in groupTransactionTypes)
            {
                var dataItem = new DataItem
                {
                    Name = group.First().transCategory.Name,
                    Amount = group.Sum(gr => gr.transactionItem.Amount),
                    Y = (group.Sum(gr => gr.transactionItem.Amount) / totalSum) * 100
                };
                
                dataItems.Add(dataItem);
            }

            return dataItems;
        }
    }
}
