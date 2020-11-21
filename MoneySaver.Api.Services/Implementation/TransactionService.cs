﻿using AutoMapper;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public TransactionModel CreateTransaction(TransactionModel transactionModel)
        {

            Transaction transaction = mapper.Map<Transaction>(transactionModel);
            this.transactionRepository.AddAsync(transaction);

            return transactionModel;
        }

        public List<TransactionModel> GetAllTransactions()
        {
            var transactions = this.transactionRepository.GetAll().ToList();
            List<TransactionModel> transactionModels = new List<TransactionModel>();

            for (int i = 0; i < transactions.Count; i++)
            {
                TransactionModel transactionModel = mapper.Map<TransactionModel>(transactions[i]);
                transactionModels.Add(transactionModel);
            }

            return transactionModels;
        }

        public TransactionModel GetTransaction(Guid id)
        {
            Transaction transaction = this.transactionRepository.GetAll().FirstOrDefault(t => t.Id == id);
            TransactionModel transactionModel = mapper.Map<TransactionModel>(transaction);

            return transactionModel;
        }

        public TransactionModel UpdateTransaction(TransactionModel transactionModel)
        {
            
            Transaction transaction = mapper.Map<Transaction>(transactionModel);
             this.transactionRepository.UpdateAsync(transaction);

            return transactionModel;
        }

        public void RemoveTransaction(Guid id)
        {
            Transaction transaction = this.transactionRepository.GetAll().FirstOrDefault(t => t.Id == id);
            transaction.IsDeleted = true;
            this.transactionRepository.RemoveAsync(transaction);
        }

    }
}
