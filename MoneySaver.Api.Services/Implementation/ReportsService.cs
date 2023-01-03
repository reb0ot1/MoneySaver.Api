using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class ReportsService : IReportsService
    {
        private readonly ILogger<ReportsService> logger;
        private readonly IRepository<Transaction> transactionRepository;
        private readonly IRepository<TransactionCategory> transactionCategoryRepository;
        private readonly UserPackage userPackage;

        public ReportsService(
            ILogger<ReportsService> logger,
            IRepository<Transaction> transactionRepository,
            IRepository<TransactionCategory> transactionCategoryRepository,
            UserPackage userPackage
            )
        {
            this.logger = logger;
            this.transactionRepository = transactionRepository;
            this.transactionCategoryRepository = transactionCategoryRepository;
            this.userPackage = userPackage;
        }

        //TODO: Add filter parameter
        public async Task<LineChartData> GetExpensesByPeriod(FilterModel filter)
        {
            var startEndDates = this.GetStartEndDateByMonthInterval(filter.From, filter.To);

            var dateTime = startEndDates.Item2.Date;
            var testDateTime = startEndDates.Item1;
            var allPeriods = new List<string>();
            while (testDateTime <= dateTime)
            {
                allPeriods.Add($"{testDateTime.Month}/{testDateTime.Year}");
                testDateTime = testDateTime.AddMonths(1);
            }

            var dataContainer = new LineChartData { Categories = allPeriods.ToArray(), Series = new List<SeriesItem>()};
            dataContainer.Series.Add(new SeriesItem { Name = "Spent ammount", Data = new double?[allPeriods.Count] });

            try
            {
                var query = this.transactionRepository
                    .GetAll()
                    .Where(e => !e.IsDeleted && e.TransactionDate >= startEndDates.Item1 && e.TransactionDate <= startEndDates.Item2)
                    .GroupBy(o => new
                    {
                        Month = o.TransactionDate.Month,
                        Year = o.TransactionDate.Year
                    })
                    .Select(g => new
                    {
                        Id = string.Format("{0}/{1}", g.Key.Month, g.Key.Year),
                        Amount = g.Sum(a => a.Amount)
                    });
                var result = await query.ToListAsync();

                for (int i = 0; i < allPeriods.Count; i++)
                {
                    var findPeriod = result.FirstOrDefault(e => e.Id == allPeriods[i]);
                    if (findPeriod != null)
                    {
                        dataContainer.Series.First().Data[i] = findPeriod.Amount;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong when trying to get expenses by period. UserId [{this.userPackage.UserId}]");
            }

            return dataContainer;
        }

        public async Task<LineChartData> GetExpensesForPeriodByCategoriesAsync(FilterModel filter)
        {
            var dataItems = new LineChartData();
            try
            {
                var startEndDates = this.GetStartEndDateByMonthInterval(filter.From, filter.To);

                var dateTime = startEndDates.Item2.Date;
                var testDateTime = startEndDates.Item1;
                var allPeriods = new List<string>();
                while (testDateTime <= dateTime)
                {
                    allPeriods.Add($"{testDateTime.Month}/{testDateTime.Year}");
                    testDateTime = testDateTime.AddMonths(1);
                }

                dataItems = new LineChartData 
                { 
                    Categories = allPeriods.ToArray(),
                    Series = new List<SeriesItem>() 
                };

                var query = from transactionItem in this.transactionRepository.GetAll()
                            join transCategory in this.transactionCategoryRepository.GetAll()
                                on transactionItem.TransactionCategoryId equals transCategory.TransactionCategoryId
                            where !transactionItem.IsDeleted &&
                            transactionItem.TransactionDate >= startEndDates.Item1 && transactionItem.TransactionDate <= startEndDates.Item2 && 
                            (!filter.CategoryIds.Any() || filter.CategoryIds.Contains(transCategory.TransactionCategoryId))
                            select new { 
                                TransId = transactionItem.Id, 
                                CategoryId = transCategory.TransactionCategoryId,
                                TransactionDate = transactionItem.TransactionDate,
                                CategoryName = transCategory.Name,
                                SpentAmount = transactionItem.Amount
                            };

                var dbResult = await query.ToListAsync();

                var result = dbResult.GroupBy(o => new {
                    Month = o.TransactionDate.Month,
                    Year = o.TransactionDate.Year
                })
                .Select(g => new { Id = string.Format("{0}/{1}", g.Key.Month, g.Key.Year), Entity = g.Select(s => s).ToList() })
                .ToList();

                var usedCategories = result
                    .SelectMany(e => e.Entity.Select(ss => new { Id = ss.CategoryId, Name = ss.CategoryName }))
                    .Distinct();

                foreach (var usedCategory in usedCategories)
                {
                    var categorySerie = new SeriesItem();
                    var data = new List<double?>();
                    for (int i = 0; i < allPeriods.Count; i++)
                    {
                        var categoryData = result.FirstOrDefault(e => e.Id == allPeriods[i] && e.Entity.Any(a => a.CategoryId == usedCategory.Id));
                        if (categoryData != null)
                        {
                            data.Add(categoryData.Entity.Where(e => e.CategoryId == usedCategory.Id).Sum(s => s.SpentAmount));
                        }
                        else
                        {
                            data.Add(null);
                        }
                    }

                    categorySerie.Name = usedCategory.Name;
                    categorySerie.Data = data.ToArray();

                    dataItems.Series.Add(categorySerie);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get expenses request. UserId {this.userPackage.UserId}", filter);
            }

            return dataItems;
        }

        public async Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter)
        {
            var dataItems = new List<DataItem>();
            try
            {
                var startEndDates = this.GetStartEndDateByMonthInterval(filter.From, filter.To);

                var query = from transactionItem in this.transactionRepository.GetAll()
                        .Where(e => !e.IsDeleted && e.TransactionDate >= startEndDates.Item1 && e.TransactionDate <= startEndDates.Item2 )
                            join transCategory in this.transactionCategoryRepository.GetAll()
                                on transactionItem.TransactionCategoryId equals transCategory.TransactionCategoryId
                            select new { transactionItem, transCategory };

                var result = await query.ToListAsync();
                var totalSum = result.Sum(tr => tr.transactionItem.Amount);
                var groupTransactionTypes = result.GroupBy(gr => gr.transactionItem.TransactionCategoryId);

                //Get all parent categories
                var parentCategories = await this.transactionCategoryRepository
                    .GetAll()
                    .Where(ct => !ct.IsDeleted && !ct.ParentId.HasValue)
                    .Select(e => new { Id = e.TransactionCategoryId, Name = e.Name })
                    .ToArrayAsync();

                foreach (var group in groupTransactionTypes)
                {
                    var firstGroupElem = group.First().transCategory;
                    var name = firstGroupElem.Name;

                    if (firstGroupElem.ParentId != null)
                    {
                        var parrentName = parentCategories.FirstOrDefault(e => e.Id == firstGroupElem.ParentId);
                        name = $"{parrentName.Name}, {firstGroupElem.Name}";
                    }

                    var dataItem = new DataItem
                    {
                        Name = name,
                        Amount = group.Sum(gr => gr.transactionItem.Amount),
                        Y = (group.Sum(gr => gr.transactionItem.Amount) / totalSum) * 100
                    };

                    dataItems.Add(dataItem);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get expenses request. UserId {this.userPackage.UserId}", filter);
            }

            return dataItems;
        }

        private (DateTime, DateTime) GetStartEndDateByMonthInterval(DateTime start, DateTime end)
        {
            var daysInMonth = DateTime.DaysInMonth(end.Year, end.Month);
            var dateEndMonth = new DateTime(end.Year, end.Month, daysInMonth);
            var dateStartMonth = new DateTime(start.Year, start.Month, 01);

            return (dateStartMonth, dateEndMonth);
        }
    }
}
