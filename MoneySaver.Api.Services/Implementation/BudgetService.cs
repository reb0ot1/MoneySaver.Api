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
        private ILogger<BudgetService> _logger;
        private UserPackage userPackage;
        private readonly IDateProvider _dateProvider;

        public BudgetService(
            IRepository<Budget> budgetRepository, 
            IRepository<BudgetItem> budgetItemRepository, 
            IRepository<TransactionCategory> transactionCategory,
            IRepository<Transaction> transactionRepository,
            IMapper mapper,
            ILogger<BudgetService> logger,
            UserPackage userPackage,
            IDateProvider dateProvider)
        {
            this.budgetRepository = budgetRepository;
            this.budgetItemRepository = budgetItemRepository;
            this.mapper = mapper;
            this.transactionCategory = transactionCategory;
            this.transactionRepository = transactionRepository;
            this._logger = logger;
            this.userPackage = userPackage;
            this._dateProvider = dateProvider;
        }

        public async Task<Result<BudgetResponseModel>> GetBudgetAsync(int id)
        {
            try
            {
                var budgetDb = await this.budgetRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (budgetDb is null)
                {
                    return $"Budget with id [{id}] not found.";
                }

                var result = mapper.Map<BudgetResponseModel>(budgetDb);

                return result;
            }
            catch (Exception ex)
            {
                var message = $"Failed to get budget with id [{id}].";
                this._logger.LogError(ex, message + $" User id [{this.userPackage.UserId}]");
                return message;
            }
        }

        public async Task<Result<PageModel<BudgetResponseModel>>> GetBudgetsPerPageAsync(int page, int pageSize)
        {
            try
            {
                var pageProps = RequestsUtility.CheckPageProperties(page, pageSize);
                var budgetsCount = await this.budgetRepository.GetAll().CountAsync();
                var budgetsResult = await this.budgetRepository
                    .GetAll()
                    .OrderByDescending(e => e.IsInUse)
                    .ThenByDescending(e => e.StartDate)
                    .Skip((pageProps.Page - 1) * pageProps.PageSize)
                    .Take(pageProps.PageSize    )
                    .Select(e => new BudgetResponseModel { 
                        BudgetType = e.BudgetType,
                        EndDate = e.EndDate,
                        StartDate = e.StartDate,
                        Id = e.Id,
                        Name = e.Name,
                        IsInUse = e.IsInUse
                    })
                    .ToListAsync();

                var resultModel = Result<PageModel<BudgetResponseModel>>
                                        .SuccessWith(new PageModel<BudgetResponseModel> 
                                        {   
                                            Result = budgetsResult,
                                            TotalCount = budgetsCount
                                        });
                return resultModel;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Failed to get budgets. UserId {this.userPackage.UserId}");
                return $"Something went wrong while trying to get all budgets.";
            }
        }

        //TODO: Change the model which needs to be returned
        //TODO: Think about if the budget should be set as IsCurrentlyInUse when coping a budget
        public async Task<Result<Budget>> CopyBudgetAsync(int budgetId, bool setToBeInUse = false)
        {
            try
            {
                var budgetDb = await this.budgetRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.Id == budgetId);

                var newDates = DateUtility.GetBudgetTypeNextDate(budgetDb.BudgetType, budgetDb.StartDate);
                var checkIfBudgetExists = await this.budgetRepository
                    .GetAll()
                    .AnyAsync(e => e.BudgetType == budgetDb.BudgetType && e.StartDate == newDates.Start && e.EndDate == newDates.End);

                if (checkIfBudgetExists)
                {
                    return $"Budget already exists for budget type [{budgetDb.BudgetType}] and date range [{newDates.Start.ToString("dd/mm/yyyy")} - {newDates.End.ToString("dd/mm/yyyy")}]";
                }

                var butgetItems = budgetDb.BudgetItems
                    .Select(s => new BudgetItem
                    {
                        LimitAmount = s.LimitAmount,
                        TransactionCategoryId = s.TransactionCategoryId,
                        UserId = s.UserId
                    })
                    .ToList();

                var newBudget = new Budget
                {
                    Name = this.CreateBudgetNameByStartDate(newDates.Start),
                    BudgetType = budgetDb.BudgetType,
                    StartDate = newDates.Start,
                    EndDate = newDates.End,
                    BudgetItems = butgetItems
                };

                if (setToBeInUse)
                {
                    var budgetCurrentlyInUse = await this.budgetRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.IsInUse);

                    budgetCurrentlyInUse.IsInUse = false;

                    await this.budgetRepository.UpdateAsync(budgetCurrentlyInUse);

                    newBudget.IsInUse = true;
                }

                //TODO: This insert should be moved to another place
                var copiedBudget =  await this.budgetRepository.AddAsync(newBudget);

                return copiedBudget;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Failed to copy budget with id {budgetId}. UserId {this.userPackage.UserId}");
                return $"Something went wrong while trying to copy budget with id [{budgetId}].";
            }
        }

        public async Task<Result<BudgetItemModel>> AddItemAsync(int budgetId, BudgetItemModel budgetItemModel)
        {
            try
            {
                var budget = await this.budgetRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.Id == budgetId);

                if (budget is null)
                {
                    return $"Budget with id [{budgetId}] does not exist.";
                }

                var category = await this.transactionCategory.GetAll().FirstOrDefaultAsync(e => e.TransactionCategoryId == budgetItemModel.TransactionCategoryId);
                if (category is null)
                {
                    return $"Category with id [{budgetItemModel.TransactionCategoryId}] does not exist.";
                }

                if (budget.BudgetItems.Any(e => e.TransactionCategoryId == budgetItemModel.TransactionCategoryId))
                {
                    return $"Category with id [{budgetItemModel.TransactionCategoryId}] already exists in budget with id [{budgetId}].";
                }

                //BudgetItem budgetItem = mapper.Map<BudgetItem>(budgetItemModel);
                BudgetItem budgetItem = new BudgetItem
                {
                    BudgetId = budgetId,
                    LimitAmount = budgetItemModel.LimitAmount,
                    TransactionCategoryId = budgetItemModel.TransactionCategoryId
                };

                budgetItem.BudgetId = budgetId;
                var result = await this.budgetItemRepository
                    .AddAsync(budgetItem);
                
                budgetItemModel.Id = result.Id;

                return budgetItemModel;
            }
            catch (Exception ex)
            {
                var message = $"Failed to add budget item. UserId {this.userPackage.UserId}";
                this._logger.LogError(ex, message, budgetItemModel);
                return message;
            }
        }

        public async Task<Result<BudgetResponseModel>> CreateBudgetAsync(CreateBudgetRequest model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return $"Invalid budget name.";
                }

                if (model.StartDate >= model.EndDate)
                {
                    return "The start date should be lower than the end date.";
                }

                var budgetDb = await this.budgetRepository
                    .GetAll()
                    .AnyAsync(e => e.StartDate == model.StartDate && e.EndDate == model.EndDate && e.Name == model.Name);

                if (budgetDb)
                {
                    return $"Budget with the requested parameters ({nameof(model.StartDate)}, {nameof(model.EndDate)} and {model.Name}) already exists.";
                }

                var modelToInsert = this.mapper.Map<BudgetEntityModel>(model);
                var result = await this.InsertBudgetAsync(modelToInsert);

                return mapper.Map<BudgetResponseModel>(result.Data);
            }
            catch (Exception ex)
            {
                var message = "Failed to create budget.";
                this._logger.LogError(ex, message + "{0}", model);
                return message;
            }
        }

        public async Task<Result<BudgetResponseModel>> UpdateBudgetAsync(int id, UpdateBudgetRequest model)
        {
            try
            {
                var budgetDb = await this.budgetRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (budgetDb is null)
                {
                    return $"Budget does not exist. id [{id}]";
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return $"Invalid budget name.";
                }

                if (model.StartDate >= model.EndDate)
                {
                    return "The start date should be lower than the end date.";
                }

                var searchForAlreadyExistingBudget = await this.budgetRepository
                    .GetAll()
                    .AnyAsync(e => e.Id != id && e.StartDate == model.StartDate && e.EndDate == model.EndDate && e.Name == model.Name);

                if (searchForAlreadyExistingBudget)
                {
                    return $"Budget with the requested parameters ({nameof(model.StartDate)}, {nameof(model.EndDate)} and {model.Name}) already exists.";
                }

                if (model.IsInUse)
                {
                    var budgetCurrentlyInUse = await this.budgetRepository.GetAll().FirstOrDefaultAsync(e => e.IsInUse);
                    if (budgetCurrentlyInUse is not null && budgetCurrentlyInUse.Id != id)
                    {
                        budgetCurrentlyInUse.IsInUse = false;
                        await this.budgetRepository.UpdateAsync(budgetCurrentlyInUse);
                    }
                }
                budgetDb.Name = model.Name;
                budgetDb.StartDate = model.StartDate;
                budgetDb.EndDate = model.EndDate;
                budgetDb.IsInUse = model.IsInUse;
                budgetDb.BudgetType = model.BudgetType;

                var result = await this.budgetRepository.UpdateAsync(budgetDb);

                return mapper.Map<BudgetResponseModel>(result);
            }
            catch (Exception ex)
            {
                var message = $"Failed to update budget. id [{id}]";
                this._logger.LogError(ex, message + "{0}", model);
                return message;
            }
        }

        public async Task<Result<BudgetItemModel>> EditItemAsync(int budgetId, int budgetItemId, BudgetItemRequestModel budgetItemModel)
        {
            try
            {
                if (budgetItemModel.LimitAmount < 0)
                {
                    return $"Limit amount cannot be lower than 0.";
                }

                var budgetItemEntity = await this.budgetItemRepository
                    .GetAll()
                    .FirstOrDefaultAsync(i => i.BudgetId == budgetId && i.Id == budgetItemId);

                if (budgetItemEntity == null)
                {
                    return $"Item with id [{budgetItemId}] not found.";
                }

                if (budgetItemEntity.TransactionCategoryId != budgetItemModel.TransactionCategoryId)
                {
                    var checkForDuplicatedCategories = await this.budgetItemRepository
                         .GetAll()
                         .AnyAsync(e => e.BudgetId == budgetId && e.TransactionCategoryId == budgetItemModel.TransactionCategoryId);

                    if (checkForDuplicatedCategories)
                    {
                        return $"Category with id [{budgetItemModel.TransactionCategoryId}] already exists in budget with id [{budgetId}]";
                    }

                    budgetItemEntity.TransactionCategoryId = budgetItemModel.TransactionCategoryId;
                }

                budgetItemEntity.LimitAmount = budgetItemModel.LimitAmount;

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
                var message = "Failed to edit budget item with id {0}. UserId {1}";
                this._logger.LogError(ex, message, budgetItemId, budgetItemModel);
                return string.Format(message, budgetItemId, budgetItemModel);
            }
        }

        public async Task<Result<IEnumerable<BudgetItemModel>>> GetBudgetItemsAsync(int budgetId)
        {
            try
            {
                var result = await this.budgetItemRepository
                    .GetAll()
                    .Where(w => w.BudgetId == budgetId)
                    .Select(s => new BudgetItemModel
                    {
                        Id = s.Id,
                        LimitAmount = s.LimitAmount,
                        TransactionCategoryId = s.TransactionCategoryId,
                        TransactionCategoryName = s.TransactionCategory.Name
                    })
                    .ToListAsync();

                if (result == null || !result.Any())
                {
                    return $"Budget items in budget with id [{budgetId}] does not exists.";
                }

                return result;
            }
            catch (Exception ex)
            {
                var message = $"Failed to gather budget items for budget with id [{budgetId}].";
                this._logger.LogError(ex, message);
                return message;
            }
        }

        public async Task<Result<bool>> RemoveItemAsync(int budgetId, int itemId)
        {
            try
            {
                BudgetItem item = await this.budgetItemRepository
                .GetAll()
                .FirstOrDefaultAsync(i => i.Id == itemId && i.BudgetId == budgetId);

                if (item is null)
                {
                    return $"Budget item with id {itemId} for budget with id {budgetId} does not exist.";
                }

                item.IsDeleted = true;

                await this.budgetItemRepository.SetAsDeletedAsync(item);

                return true;
            }
            catch (Exception ex)
            {
                var message = $"Failed to remove item with id [{itemId}] from budget with id [{budgetId}].";
                this._logger.LogError(ex, message);

                return message;
            }
        }

        public async Task<Result<BudgetResponseModel>> GetCurrentInUseAsync()
        {
            try
            {
                var budgetInUseRequest = await this.budgetRepository.GetAll()
                .Where(e => e.IsInUse)
                .ToListAsync();

                if (budgetInUseRequest.Count > 1)
                {
                    this._logger.LogError($"There are more than one budget in use. userId [{this.userPackage.UserId}]");
                    return "There are more than one budget in use.";
                }

                //TODO: The below should be moved from this method.
                //if (budgetInUseRequest.Count == 0)
                //{
                //    var dateTimeNow = this._dateProvider.GetDateTimeNow();
                //    var getMonthBudgetDates = DateUtility.GetBudgetTypeNextDate(BudgetType.Monthly, dateTimeNow.AddMonths(-1));

                //    var createdBudget = await this.InsertBudgetAsync(new BudgetEntityModel
                //    {
                //        BudgetItems = new List<BudgetItemModel>(),
                //        BudgetType = BudgetType.Monthly,
                //        StartDate = getMonthBudgetDates.Start,
                //        EndDate = getMonthBudgetDates.End,
                //        IsInUse = true,
                //        Name = this.CreateBudgetNameByStartDate(getMonthBudgetDates.Start)
                //    });

                //    return mapper.Map<BudgetResponseModel>(createdBudget.Data);
                //}

                //TODO: This is added to check if a new budget item is needed to be copied. This will be changed when the new feature for budget is implemented
                //var dateTimeNow = this._dateProvider.GetDateTimeNow();
                //if (budgetInUse.EndDate < dateTimeNow)
                //{
                //    var newBudgetResult = await this.CopyBudgetAsync(budgetInUse.Id, true);
                //    if (newBudgetResult.Succeeded)
                //    {
                //        budgetInUse.IsCurenttlyInUse = false;
                //        await this.budgetRepository
                //            .UpdateAsync(budgetInUse);

                //        return mapper.Map<BudgetResponseModel>(newBudgetResult.Data);
                //    }
                //}

                var result = mapper.Map<BudgetResponseModel>(budgetInUseRequest.First());

                return result;
            }
            catch (Exception ex)
            {
                var message = $"Failed to get budget in use.";
                this._logger.LogError(ex, message + $"User id [{this.userPackage.UserId}].");
                return message;
            }
        }

        public async Task<Result<IEnumerable<BudgetItemSpentModel>>> GetSpentAmountsAsync(int budgetId)
        {
            try
            {
                var budgetItemsQuery = from budgetItem in this.budgetItemRepository.GetAll()
                                       join budget in this.budgetRepository.GetAll()
                                       on budgetItem.BudgetId equals budget.Id
                                       join categoryItem in this.transactionCategory.GetAll()
                                       on budgetItem.TransactionCategoryId equals categoryItem.TransactionCategoryId
                                       where budgetItem.BudgetId == budgetId
                                       select new
                                       {
                                           BudgetId = budgetItem.Id,
                                           BudgetItemAmount = budgetItem.LimitAmount,
                                           CategoryId = budgetItem.TransactionCategoryId,
                                           CategoryName = categoryItem.Name,
                                           BudgetStartDate = budget.StartDate,
                                           BudgetEndDate = budget.EndDate
                                       };

                var result = await budgetItemsQuery.ToListAsync();
                if (result == null || !result.Any())
                {
                    return new List<BudgetItemSpentModel>();
                }

                var budgetEntity = result.FirstOrDefault();
                var budgetDb = new BudgetEntity(
                    budgetId,
                    budgetEntity.BudgetStartDate,
                    budgetEntity.BudgetEndDate,
                    result.Select(e => new BudgetItemEntity(e.BudgetId, e.CategoryId, e.CategoryName, e.BudgetItemAmount))
                          .ToList()
                    );

                var spentAmountsByCategory = await this.GetSpentAmountsbyCategory(budgetDb);

                return this.GetBudgetItemsResult(budgetDb.BudgetItems, spentAmountsByCategory) as List<BudgetItemSpentModel>;
            }
            catch (Exception ex)
            {
                var message = $"Failed to gather budget items for budget with id [{budgetId}].";
                this._logger.LogError(ex, message + $"User id [{this.userPackage.UserId}]");
                return message;
            }
        }

        private async Task<IEnumerable<SpentAmountByCategory>> GetSpentAmountsbyCategory(BudgetEntity budgetDb)
           => await this.transactionRepository.GetAll()
                   .Where(w => w.TransactionDate >= budgetDb.StartDate && w.TransactionDate <= budgetDb.EndDate)
                   .GroupBy(gr => gr.TransactionCategoryId)
                   .Select(g => new SpentAmountByCategory(g.Key, g.Sum(s => s.Amount)))
                   .ToListAsync();

        private IEnumerable<BudgetItemSpentModel> GetBudgetItemsResult(
            IEnumerable<BudgetItemEntity> budgetDbItems, 
            IEnumerable<SpentAmountByCategory> spentAmountsByCategory
            )
        {
            var result = new List<BudgetItemSpentModel>();
            foreach (var budgetItem in budgetDbItems)
            {
                SpentAmountByCategory? spentAmountByCategory = spentAmountsByCategory
                        .FirstOrDefault(w => w.CategoryId == budgetItem.CategoryId);

                if (spentAmountByCategory is null)
                {
                    continue;
                }

                var budgetItemModel = new BudgetItemSpentModel
                {
                    Id = budgetItem.Id,
                    TransactionCategoryId = budgetItem.CategoryId,
                    TransactionCategoryName = budgetItem.CategoryName,
                    LimitAmount = budgetItem.LimitAmount,
                    SpentAmount = spentAmountByCategory.Value.Amount
                };

                //TODO: This should be removed. Calculate progress should be in the SPA app
                budgetItemModel.CalculateProgress();

                result.Add(budgetItemModel);
            };

            return result;
        }

        private Result<bool> ValidateBudgetDate(BudgetType budgetType, DateTime startDate)
        {
            if (budgetType == BudgetType.Monthly)
            {
                if (startDate.Day != 1)
                {
                    return $"The start date should be the first day of the month.";
                }
            }

            if (budgetType == BudgetType.Yearly)
            {
                if (startDate.Month != 1)
                {
                    return $"The start date should be the first month of the year.";
                }

                if (startDate.Day != 1)
                {
                    return $"The start date should be the first day of the month.";
                }
            }

            if (budgetType == BudgetType.Weekly)
            {
                if (startDate.DayOfWeek != DayOfWeek.Monday)
                {
                    return $"The start date should be the first day of the week.";
                }
            }

            return true;
        }

        private async Task<Result<BudgetEntityModel>> InsertBudgetAsync(BudgetEntityModel budgetModel)
        {
            var modelToInsert = this.mapper.Map<Budget>(budgetModel);
            var result = await this.budgetRepository.AddAsync(modelToInsert);

            return mapper.Map<BudgetEntityModel>(result);
        }

        private string CreateBudgetNameByStartDate(DateTime startDate)
            => startDate.ToString("MM") + "-" + startDate.ToString("yyy");

        private readonly record struct SpentAmountByCategory(int CategoryId, double Amount);
        private readonly record struct BudgetEntity(int Id, DateTime StartDate, DateTime EndDate, IEnumerable<BudgetItemEntity> BudgetItems);
        private readonly record struct BudgetItemEntity(int Id, int CategoryId, string CategoryName, double LimitAmount);
    }
}
