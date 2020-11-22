using AutoMapper;
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
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private IRepository<TransactionCategory> categoryRepository;
        private IMapper mapper;

        public TransactionCategoryService(IRepository<TransactionCategory> categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public TransactionCategoryModel CreateCategory(TransactionCategoryModel categoryModel)
        {
            TransactionCategory transactionCategory = mapper.Map<TransactionCategory>(categoryModel);
            this.categoryRepository.AddAsync(transactionCategory);

            return categoryModel;
        }

        public List<TransactionCategoryModel> GetAllCategories()
        {
            List<TransactionCategoryModel> transactionCategoryModels = categoryRepository.GetAll().Select(m => mapper.Map<TransactionCategoryModel>(m)).ToList();
            return transactionCategoryModels;
        }

        public TransactionCategoryModel GetCategory(int id)
        {
            TransactionCategory transactionCategory = this.categoryRepository.GetAll().FirstOrDefault(c => c.TransactionCategoryId == id);
            TransactionCategoryModel transactionCategoryModel = mapper.Map<TransactionCategoryModel>(transactionCategory);

            return transactionCategoryModel;
        }

        public void RemoveCategory(int id)
        {
            TransactionCategory transactionCategory = this.categoryRepository.GetAll().FirstOrDefault(c => c.TransactionCategoryId == id);
            transactionCategory.IsDeleted = true;
            transactionCategory.DeletedOnUtc = DateTime.UtcNow;
            this.categoryRepository.RemoveAsync(transactionCategory);
        }

        public TransactionCategoryModel UpdateCategory(TransactionCategoryModel categoryModel)
        {
            TransactionCategory transactionCategory = mapper.Map<TransactionCategory>(categoryModel);
            this.categoryRepository.UpdateAsync(transactionCategory);

            return categoryModel;
        }
    }
}
