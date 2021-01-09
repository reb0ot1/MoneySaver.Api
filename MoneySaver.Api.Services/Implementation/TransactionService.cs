using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private IRepository<Transaction> transactionRepository;
        private IMapper mapper;

        public TransactionService(IRepository<Transaction> transactionRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.mapper = mapper;
        }

        public async Task<TransactionModel> CreateTransactionAsync(TransactionModel transactionModel, string userId)
        {
            try
            {
                Transaction transaction = mapper.Map<Transaction>(transactionModel);
                transaction.UserId = userId;
                await this.transactionRepository.AddAsync(transaction);

                return transactionModel;
            }
            catch
            {
                ;
            }

            return null;
        }

        public async Task<IEnumerable<TransactionModel>> GetAllTransactionsAsync()
        {
            try
            {
                List<TransactionModel> transactionModels = await this.transactionRepository.GetAll().Select(m => mapper.Map<TransactionModel>(m)).ToListAsync();

                return transactionModels;
            }
            catch
            {
                ;
            }

            return null;
        }

        public async Task<TransactionModel> GetTransactionAsync(Guid id)
        {
            try
            {
                Transaction transaction = await this.transactionRepository.GetAll().FirstOrDefaultAsync(t => t.Id == id);
                TransactionModel transactionModel = mapper.Map<TransactionModel>(transaction);

                return transactionModel;
            }
            catch
            {
                ;
            }

            return null;
        }

        public async Task<TransactionModel> UpdateTransactionAsync(TransactionModel transactionModel)
        {
            try
            {
                Transaction transaction = mapper.Map<Transaction>(transactionModel);
                await this.transactionRepository.UpdateAsync(transaction);

                return transactionModel;
            }
            catch
            {
                ;
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
                await this.transactionRepository.RemoveAsync(transaction);
            }
            catch
            {
                ;
            }
        }

    }
}
