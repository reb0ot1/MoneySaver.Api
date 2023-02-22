using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Utilities;
using MoneySaver.System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class BudgetService : IBudgetService
    {
        private IRepository<BudgetItem> budgetItemRepository;
        private IRepository<Budget> budgetRepository;
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

        public async Task<Result<Budget>> CopyBudgetAsync(int budgetId, bool setToBeInUse = false)
        {
            try
            {
                var budgetDb = await this.budgetRepository.GetAll().FirstOrDefaultAsync(e => e.Id == budgetId);

                var newDates = DateUtility.GetBudgetTypeNextDate(budgetDb.BudgetType, budgetDb.StartDate);
                var checkIfBudgetExists = await this.budgetRepository
                    .GetAll()
                    .AnyAsync(e => e.BudgetType == budgetDb.BudgetType && e.StartDate == newDates.Start && e.EndDate == newDates.End);

                if (checkIfBudgetExists)
                {
                    return $"Budget already exists for budget type [{budgetDb.BudgetType}] and date range [{newDates.Start.ToString("dd/mm/yyyy")} - {newDates.End.ToString("dd/mm/yyyy")}]";
                }
                
                var butgetItems = await this.budgetItemRepository
                    .GetAll()
                    .Where(e => e.BudgetId == budgetId)
                    .Select(s => new BudgetItem
                    {
                        LimitAmount = s.LimitAmount,
                        TransactionCategoryId = s.TransactionCategoryId,
                        UserId = s.UserId
                    })
                    .ToListAsync();

                var newBudget = new Budget
                {
                    Name = budgetDb.Name,
                    BudgetType = budgetDb.BudgetType,
                    StartDate = newDates.Start,
                    EndDate = newDates.End,
                    BudgetItems = butgetItems,
                    IsCurenttlyInUse = setToBeInUse
                };

                var copiedBudget =  await this.budgetRepository.AddAsync(newBudget);

                return copiedBudget;
            }
            catch (Exception ex)
            {

                this.logger.LogError(ex, $"Failed to copy budget with id {budgetId}. UserId {this.userPackage.UserId}");
                return $"Something went wrong while trying to copy budget with id [{budgetId}].";
            }
        }

        public async Task<BudgetItemModel> AddItemAsync(int budgetId, BudgetItemModel budgetItemModel)
        {
            try
            {
                //TODO: Check if budget exists
                //TODO: Check if category exists
                BudgetItem budgetItem = mapper.Map<BudgetItem>(budgetItemModel);
                budgetItem.BudgetId = budgetId;
                budgetItem.TransactionCategory = null;
                var result = await this.budgetItemRepository.AddAsync(budgetItem);
                budgetItemModel.Id = result.Id;

                return budgetItemModel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to add budget item. UserId {this.userPackage.UserId}", budgetItemModel);
            }

            //TODO: Create class which will handle the result of the services
            return null;
        }

        public async Task<BudgetResponseModel> CreateBudget(CreateBudgetRequest model)
        {
            //TODO: Validate request model

            var budgetDates = DateUtility.GetPeriodByBudgetType(model.BudgetType, model.StartDate);
            var budgetDb = await this.budgetRepository.GetAll()
                .FirstOrDefaultAsync(e => e.StartDate == budgetDates.Start && e.BudgetType == model.BudgetType);

            if (budgetDb != null)
            {
                return null;
            }

            var modelToInsert = new Budget
            {
                Name = model.Name,
                BudgetType = model.BudgetType,
                StartDate = budgetDates.Start,
                EndDate = budgetDates.End,
                BudgetItems = new List<BudgetItem>()
            };

            if (model.CopyPrevItems)
            { 
                //TODO: Get the lattest budget from the same type and get its categories and add them for the new budget.
            }

            var result = await this.budgetRepository.AddAsync(modelToInsert);

            //TODO: Use automapper
            return new BudgetResponseModel
            {
                Id = result.Id,
                BudgetType = result.BudgetType,
                Name = result.Name,
                EndDate = result.EndDate,
                StartDate = result.StartDate
            };
        }

        public async Task<BudgetItemModel> EditItemAsync(int budgetId, int budgetItemId, BudgetItemRequestModel budgetItemModel)
        {
            //TODO: validate the request model values
            try
            {
                var budgetItemEntity = await this.budgetItemRepository
                    .GetAll()
                    .FirstOrDefaultAsync(i => i.BudgetId == budgetId && i.Id == budgetItemId);

                if (budgetItemEntity == null)
                {
                    return null;
                }

                budgetItemEntity.LimitAmount = budgetItemModel.LimitAmount;
                budgetItemEntity.TransactionCategoryId = budgetItemModel.TransactionCategoryId;

                var result = await this.budgetItemRepository.UpdateAsync(budgetItemEntity);

                return new BudgetItemModel 
                {
                    Id = budgetItemId,
                    LimitAmount = budgetItemModel.LimitAmount,
                    TransactionCategoryId = budgetItemModel.TransactionCategoryId,
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to edit budget item with id {0}. UserId {1}", budgetItemId, budgetItemModel);
            }

            return null;
        }

        //TODO: Add filter object in the parametters
        public async Task<BudgetModel> GetBudgetItems()
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


        public async Task<IEnumerable<BudgetItemModel>> GetBudgetItemsAsync(int budgetId)
        {
            var budgetDb = await this.budgetRepository.GetAll().FirstOrDefaultAsync(e => e.Id == budgetId);
            if (budgetDb == null)
            {
                throw new Exception("budget not found.");
            }

            //TODO: Think a better way to get the budget items and their transactions
            var query = from budgetItem in this.budgetItemRepository.GetAll().Where(e => e.BudgetId == budgetId)
                        join trans in this.transactionCategory.GetAll().Where(e => !e.IsDeleted)
                            on budgetItem.TransactionCategoryId equals trans.TransactionCategoryId
                        select new { budgetItem, trans };

            var result = await query.ToListAsync();
            var transactionCategoryIds = result.Select(s => s.trans.TransactionCategoryId);

            var transactions = await this.transactionRepository
                .GetAll()
                .Where(w => transactionCategoryIds.Contains(w.TransactionCategoryId) &&
                            w.TransactionDate >= budgetDb.StartDate &&
                            w.TransactionDate <= budgetDb.EndDate)
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

            return budgetItems;

            //TODO: Remove the below, it should return only budget items
            //var budgetModel = new BudgetModel
            //{
            //    BudgetItems = budgetItems.OrderBy(o => o.TransactionCategoryName).ToList(),
            //    LimitAmount = budgetItems.Sum(s => s.LimitAmount),
            //    TotalSpentAmmount = budgetItems.Sum(s => s.SpentAmount),
            //};

            //budgetModel.TotalLeftAmount = budgetModel.LimitAmount - budgetModel.TotalSpentAmmount;

            //return budgetModel;
        }

        public async Task RemoveItemAsync(int budgetId, int itemId)
        {
            BudgetItem item = this.budgetItemRepository.GetAll().FirstOrDefault(i => i.Id == itemId && i.BudgetId == budgetId);
            item.IsDeleted = true;

            await this.budgetItemRepository.SetAsDeletedAsync(item);
        }

        public async Task<BudgetResponseModel> GetCurrentInUseAsync()
        {
            var budgetInUse = await this.budgetRepository
                .GetAll()
                .FirstOrDefaultAsync(e => e.IsCurenttlyInUse);
            
            //TODO: This is added to check if a new budget item is needed to be copied. This will be changed when the new feature for budget is implemented
            var dateTimeNow = DateTime.UtcNow;

            if (budgetInUse.EndDate < dateTimeNow)
            {
                var newBudgetResult = await this.CopyBudgetAsync(budgetInUse.Id, true);
                if (newBudgetResult.Succeeded)
                {
                    budgetInUse.IsCurenttlyInUse = false;
                    await this.budgetRepository.UpdateAsync(budgetInUse);

                    return mapper.Map<BudgetResponseModel>(newBudgetResult.Data);
                }
            }

            return mapper.Map<BudgetResponseModel>(budgetInUse);
        }
    }
}
