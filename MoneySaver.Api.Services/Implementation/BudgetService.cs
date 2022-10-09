using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class BudgetService : IBudgetService
    {
        private IRepository<Budget> budgetRepository;
        private IRepository<BudgetItem> budgetItemRepository;
        private IRepository<Transaction> transactionRepository;
        private IMapper mapper;
        private IRepository<TransactionCategory> transactionCategory;
        private ILogger<BudgetService> logger;
        private UserPackage userPackage;

        public BudgetService(
            IRepository<Budget> budgetRepository, 
            IRepository<BudgetItem> budgetItemRepository, 
            IRepository<TransactionCategory> transactionCategory,
            IRepository<Transaction> transactionRepository,
            IMapper mapper,
            ILogger<BudgetService> logger,
            UserPackage userPackage)
        {
            this.budgetRepository = budgetRepository;
            this.budgetItemRepository = budgetItemRepository;
            this.mapper = mapper;
            this.transactionCategory = transactionCategory;
            this.transactionRepository = transactionRepository;
            this.logger = logger;
            this.userPackage = userPackage;
        }

        public async Task<BudgetItemModel> AddItemAsync(BudgetItemModel budgetItemModel)
        {
            try
            {
                BudgetItem budgetItem = mapper.Map<BudgetItem>(budgetItemModel);
                var result = await this.budgetItemRepository.AddAsync(budgetItem);
                budgetItemModel.Id = result.Id;

                return budgetItemModel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to add budget item. UserId {this.userPackage.UserId}", budgetItemModel);
            }

            return null;
        }

        public async Task<BudgetItemModel> EditItemAsync(BudgetItemModel budgetItemModel)
        {
            //TODO: validate the request model values
            try
            {
                var budgetItemEntity = await this.budgetItemRepository
                .GetAll()
                .FirstOrDefaultAsync(i => i.Id == budgetItemModel.Id);

                if (budgetItemEntity == null)
                {
                    return null;
                }

                budgetItemEntity.LimitAmount = budgetItemModel.LimitAmount;
                budgetItemEntity.TransactionCategoryId = budgetItemModel.TransactionCategoryId;

                var result = await this.budgetItemRepository.UpdateAsync(budgetItemEntity);

                return budgetItemModel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to edit budget item with id {budgetItemModel.Id}. UserId {this.userPackage.UserId}", budgetItemModel);
            }

            return null;
        }

        //public BudgetModel CreateBudget(BudgetModel budgetModel)
        //{
        //    Budget budget = mapper.Map<Budget>(budgetModel);
        //    this.budgetRepository.AddAsync(budget);

        //    return budgetModel;
        //}

        //public List<BudgetModel> GetAllBudgets()
        //{
        //    List<BudgetModel> budgetModels = budgetRepository.GetAll().Select(m => mapper.Map<BudgetModel>(m)).ToList();

        //    return budgetModels;
        //}

        //public BudgetModel GetBudget(int id)
        //{
        //    Budget budget = this.budgetRepository
        //        .GetAll()
        //        .FirstOrDefault(b => b.Id == id);

        //    BudgetModel budgetModel = mapper.Map<BudgetModel>(budget);

        //    return budgetModel;
        //}

        //TODO: Add filter object in the parametters
        public async Task<BudgetModel> GetBudgetItems(Models.BudgetType budgetType)
        {
            //TODO: Think a better way to get the budget items and their transactions
            var query = from budgetItem in this.budgetItemRepository.GetAll().Where(e => !e.IsDeleted)
                        join trans in this.transactionCategory.GetAll().Where(e => !e.IsDeleted)
                            on budgetItem.TransactionCategoryId equals trans.TransactionCategoryId
                        select new { budgetItem, trans };

            var result = await query.ToListAsync();
            var transactionCategoryIds = result.Select(s => s.trans.TransactionCategoryId);
            var now = DateTime.UtcNow;
            var firstDayOfTheMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfTheMonth = firstDayOfTheMonth.AddMonths(1).AddTicks(-1);

            var transactions = await this.transactionRepository
                .GetAll()
                .Where(w => !w.IsDeleted &&
                            w.TransactionDate >= firstDayOfTheMonth &&
                            w.TransactionDate <= lastDayOfTheMonth)
                .Select(s => new { TransactionCategoryId = s.TransactionCategoryId, Amount = s.Amount })
                .ToListAsync();

            List<BudgetItemModel> budgetItems = new List<BudgetItemModel>();
            foreach (var item in result)
            {
                var spentAmmount = transactions
                        .Where(w => w.TransactionCategoryId == item.budgetItem.TransactionCategoryId)
                        .Select(s => s.Amount)
                        .Sum(m => m);

                var budgetItemModel = new BudgetItemModel
                {
                    Id = item.budgetItem.Id,
                    TransactionCategoryId = item.trans.TransactionCategoryId,
                    TransactionCategoryName = item.trans.Name,
                    LimitAmount = item.budgetItem.LimitAmount,
                    SpentAmount = spentAmmount
                };

                budgetItemModel.CalculateProgress();

                budgetItems.Add(budgetItemModel);
            };

            var budgetModel = new BudgetModel
            {
                BudgetItems = budgetItems.OrderBy(o => o.TransactionCategoryName).ToList(),
                LimitAmount = budgetItems.Sum(s => s.LimitAmount),
                TotalSpentAmmount = budgetItems.Sum(s => s.SpentAmount),
            };

            budgetModel.TotalLeftAmount = budgetModel.LimitAmount - budgetModel.TotalSpentAmmount;

            return budgetModel;
        }

        //public void RemoveBudget(int id)
        //{
        //    Budget budget = this.budgetRepository.GetAll().FirstOrDefault(b => b.Id == id);
        //    budget.IsDeleted = true;
        //    budget.DeletedOnUtc = DateTime.UtcNow;
        //    this.budgetRepository.RemoveAsync(budget);
        //}

        //public BudgetModel UpdateBudget(BudgetModel budgetModel)
        //{
        //    Budget budget = mapper.Map<Budget>(budgetModel);
        //    this.budgetRepository.UpdateAsync(budget);

        //    return budgetModel;
        //}

        public async Task RemoveItemAsync(int id)
        {
            BudgetItem item = this.budgetItemRepository.GetAll().FirstOrDefault(i => i.Id == id);
            item.IsDeleted = true;
            await this.budgetItemRepository.SetAsDeletedAsync(item);
        }
    }
}
