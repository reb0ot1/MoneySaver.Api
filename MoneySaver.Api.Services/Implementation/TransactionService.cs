using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using MoneySaver.Api.Models.Shared;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction> transactionRepository;
        private readonly IRepository<TransactionCategory> transactionCategoryRepository;
        private readonly IMapper mapper;
        private readonly ILogger<TransactionService> logger;
        private readonly UserPackage userPackage;

        public TransactionService(
            IRepository<Transaction> transactionRepository,
            IRepository<TransactionCategory> transactionCategoryRepository,
            IMapper mapper, 
            ILogger<TransactionService> logger,
            UserPackage userPackage)
        {
            this.transactionRepository = transactionRepository;
            this.transactionCategoryRepository = transactionCategoryRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.userPackage = userPackage;
        }

        public async Task<TransactionModel> CreateTransactionAsync(TransactionModel transactionModel)
        {
            try
            {
                Transaction transaction = mapper.Map<Transaction>(transactionModel);
                var result = await this.transactionRepository.AddAsync(transaction);
                transactionModel.Id = result.Id.ToString();
                return transactionModel;
            }
            catch(Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create transaction. UserId {this.userPackage.UserId}", transactionModel);
            }

            return null;
        }

        public async Task<PageModel<TransactionModel>> GetTransactionsForPageAsync(PageRequest pageRequest)
        {
            var result = new PageModel<TransactionModel>();

            try
            {
                var transactionsQuery = this.FilterBySearchContent(this.transactionRepository
                                                                    .GetAll()
                                                                    .OrderByDescending(e => e.TransactionDate),
                                                                   pageRequest.Filter.SearchText);

                var totalRecords = await transactionsQuery
                    .CountAsync();

                var transactionsFoundResult = await transactionsQuery
                        .Skip(pageRequest.ItemsToSkip)
                        .Take(pageRequest.ItemsPerPage)
                        .ToListAsync();


                result.Result = transactionsFoundResult;
                result.TotalCount = totalRecords;
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex, 
                    $"Failed to get the transactions for the requested page. UserId [{this.userPackage.UserId}]"
                    );
            }

            return result;
        }

        public async Task<TransactionModel> GetTransactionAsync(Guid id)
        {
            try
            {
                Transaction transaction = await this.transactionRepository.GetAll().FirstOrDefaultAsync(t => t.Id == id);
                TransactionModel transactionModel = mapper.Map<TransactionModel>(transaction);

                return transactionModel;
            }
            catch(Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get transaction with id {id}. UserId {this.userPackage.UserId}");
            }

            return null;
        }

        public async Task<TransactionModel> UpdateTransactionAsync(TransactionModel transactionModel)
        {
            //TODO: validate the request model values
            try
            {
                Guid validId;
                if (!Guid.TryParse(transactionModel.Id, out validId))
                {
                    this.logger.LogWarning($"Not a valid transaction id. [{transactionModel.Id}]");
                    return null;
                }

                Transaction transactionEntity = await this
                    .transactionRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.Id == validId);

                if (transactionEntity == null)
                {
                    this.logger.LogWarning($"No such entity found for update. id [{transactionModel.Id}]");
                    return null;
                }
                
                transactionEntity.AdditionalNote = transactionModel.AdditionalNote;
                transactionEntity.Amount = transactionModel.Amount;
                transactionEntity.TransactionCategoryId = transactionModel.TransactionCategoryId;
                transactionEntity.TransactionDate = transactionModel.TransactionDate;

                await this.transactionRepository.UpdateAsync(transactionEntity);

                return transactionModel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex, 
                    $"Failed to update transaction with id {transactionModel.Id}. UserId {this.userPackage.UserId}", 
                    transactionModel);
            }

            return null;
        }

        public async Task RemoveTransactionAsync(Guid id)
        {
            try
            {
                Transaction transaction = await this.transactionRepository
                    .GetAll()
                    .FirstOrDefaultAsync(t => t.Id == id);

                transaction.IsDeleted = true;
                transaction.DeletedOnUtc = DateTime.UtcNow;
                await this.transactionRepository.SetAsDeletedAsync(transaction);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    $"Failed to remove transaction with id {id}. UserId {this.userPackage.UserId}"
                    );
            }
        }

        private IQueryable<TransactionModel> FilterBySearchContent(IQueryable<Transaction> transQuery, string searchText)
        {
            if (searchText == null)
            {
                return transQuery.Select(e => this.mapper.Map<TransactionModel>(e));
            }

            return transQuery.Join(this.transactionCategoryRepository.GetAll(),
                    trans => trans.TransactionCategoryId,
                    categ => categ.TransactionCategoryId,
                    (trans, categ) => new
                    {
                        TransactionId = trans.Id,
                        TransactionDate = trans.TransactionDate,
                        TransactionCategoryId = trans.TransactionCategoryId,
                        Amount = trans.Amount,
                        AdditionalNote = trans.AdditionalNote,
                        CategoryName = categ.Name
                    })
                .Where(tr => tr.AdditionalNote.Contains(searchText) || tr.CategoryName == searchText)
                .Select(e => new TransactionModel
                {
                    Id = e.TransactionId.ToString(),
                    TransactionDate = e.TransactionDate,
                    AdditionalNote = e.AdditionalNote,
                    Amount = e.Amount,
                    TransactionCategoryId = e.TransactionCategoryId
                });
        }

        public async Task<IEnumerable<IdValue<double?>>> SpentAmountPerCategorieAsync(Models.BudgetType budgetType, int? itemsToTake = null)
        {
            var datePeriodToSearch = DateUtility.GetPeriodByBudgetType(budgetType, DateTime.UtcNow);
            var query = this.transactionRepository
                    .GetAll()
                    .Where(e => !e.IsDeleted && e.TransactionDate >= datePeriodToSearch.Start && e.TransactionDate <= datePeriodToSearch.End)
                    .GroupBy(gb => gb.TransactionCategoryId)
                    .Select(g => new IdValue<double?>
                    {
                        Id = g.Key,
                        Value = g.Sum(a => a.Amount)
                    });

            var result = await query
                .ToListAsync();

            return result;
        }
    }
}
