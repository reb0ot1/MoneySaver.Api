﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Shared;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Utilities;
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
        //TODO: Change the return type
        public async Task<LineChartData> GetExpensesByPeriod(FilterModel filter)
        {
            //TODO: By default the resulst are in montly time frame. Should change that.
            //TODO: Get the budget in use
            var startEndDates = DateUtility.GetStartEndDateByMonthInterval(filter.From, filter.To);

            var dateTime = startEndDates.End.Date;
            var mutableDateTime = startEndDates.Start;
            var allPeriods = new List<string>();
            while (mutableDateTime <= dateTime)
            {
                allPeriods.Add($"{mutableDateTime.Month}/{mutableDateTime.Year}");
                mutableDateTime = mutableDateTime.AddMonths(1);
            }

            var dataContainer = new LineChartData { Categories = allPeriods.ToArray(), Series = new List<SeriesItem>()};
            dataContainer.Series.Add(new SeriesItem { Name = "Spent ammount", Data = new double?[allPeriods.Count] });

            try
            {
                var query = this.transactionRepository
                    .GetAll()
                    .Where(e => e.TransactionDate >= startEndDates.Item1 && e.TransactionDate <= startEndDates.Item2)
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

        //TODO: Change the return type
        public async Task<LineChartData> GetExpensesForPeriodByCategoriesAsync(FilterModel filter)
        {
            var dataItems = new LineChartData();
            try
            {
                var startEndDates = DateUtility.GetStartEndDateByMonthInterval(filter.From, filter.To);

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
                    var data = new double?[allPeriods.Count];
                    for (int i = 0; i < allPeriods.Count; i++)
                    {
                        var categoryData = result.FirstOrDefault(e => e.Id == allPeriods[i] && e.Entity.Any(a => a.CategoryId == usedCategory.Id));
                        if (categoryData != null)
                        {
                            data[i] = categoryData.Entity
                                .Where(e => e.CategoryId == usedCategory.Id)
                                .Sum(s => s.SpentAmount);
                            //data.Add(categoryData.Entity.Where(e => e.CategoryId == usedCategory.Id).Sum(s => s.SpentAmount));
                        }
                        //else
                        //{
                        //    data.Add(null);
                        //}
                    }

                    categorySerie.Name = usedCategory.Name;
                    categorySerie.Data = data;

                    dataItems.Series.Add(categorySerie);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get expenses request. UserId {this.userPackage.UserId}", filter);
            }

            return dataItems;
        }

        //TODO: Change the return type
        public async Task<System.Services.Result<List<DataItem>>> GetExpensesPerCategoryAsync(FilterModel filter)
        {
            try
            {
                var dataItems = new List<DataItem>();
                var startEndDates = DateUtility.GetStartEndDateByMonthInterval(filter.From, filter.To);

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

                return dataItems;
            }
            catch (Exception ex)
            {
                var message = "Failed to get expenses request.";
                this.logger.LogError(ex, message + " UserId {0}. Filters: {1}", this.userPackage.UserId, filter);

                return message;
            }
        }

        //TODO: Change the return type
        public async Task<IEnumerable<IdValue<double?>>> SpentAmountPerCategorieAsync(PageRequest pageRequest)
        {
            try
            {
                var query = this.transactionRepository
                    .GetAll()
                    .Where(e => e.TransactionDate >= pageRequest.Filter.From && e.TransactionDate <= pageRequest.Filter.To)
                    .GroupBy(gb => gb.TransactionCategoryId)
                    .Select(g => new IdValue<double?>
                    {
                        Id = g.Key,
                        Value = g.Sum(a => a.Amount)
                    })
                    .OrderByDescending(e => e.Value);

                var result = await query.ToListAsync();

                if (pageRequest.ItemsPerPage > 0)
                {
                    return result.Take(pageRequest.ItemsPerPage);
                }
            }
            catch (Exception ex)
            {
                var message = "Failed to get spent amount per category.";
                this.logger.LogError(ex, message);
            }

            return new List<IdValue<double?>>();
        }
    }
}
