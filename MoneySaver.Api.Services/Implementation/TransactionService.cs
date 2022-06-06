using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction> transactionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<TransactionService> logger;
        private readonly UserPackage userPackage;

        public TransactionService(
            IRepository<Transaction> transactionRepository, 
            IMapper mapper, 
            ILogger<TransactionService> logger,
            UserPackage userPackage)
        {
            this.transactionRepository = transactionRepository;
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

        public async Task<IEnumerable<TransactionModel>> GetAllTransactionsAsync()
        {
            try
            {
                List<TransactionModel> transactionModels = await this.transactionRepository
                    .GetAll()
                    .Where(t => !t.IsDeleted)
                    .Select(m => mapper.Map<TransactionModel>(m))
                    .ToListAsync();

                return transactionModels;
            }
            catch(Exception ex)
            {
                this.logger.LogError(ex, $"Failed to gather transactions. UserId {this.userPackage.UserId}");
            }

            return null;
        }

        public async Task<PageModel<TransactionModel>> GetTransactionsForPageAsync(PageRequest pageRequest)
        {
            try
            {
                int total = await this.transactionRepository
                    .GetAll()
                    .Where(t => !t.IsDeleted)
                    .CountAsync();

                List<TransactionModel> transactionModels = await this.transactionRepository
                        .GetAll()
                        .Where(t => !t.IsDeleted)
                        .OrderByDescending(tr => tr.TransactionDate)
                        .Skip(pageRequest.ItemsToSkip)
                        .Take(pageRequest.ItemsPerPage)
                        .Select(m => mapper.Map<TransactionModel>(m))
                        .ToListAsync();

                return new PageModel<TransactionModel>()
                {
                    Result = transactionModels,
                    TotalCount = total
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get the transactions for the requested page. UserId [{this.userPackage.UserId}]");
            }
            
            return new PageModel<TransactionModel>
            {
                Result = new List<TransactionModel>(),
                TotalCount = 0
            };
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
    }
}
