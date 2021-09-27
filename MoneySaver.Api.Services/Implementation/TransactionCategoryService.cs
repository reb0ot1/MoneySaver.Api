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
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private readonly IRepository<TransactionCategory> categoryRepository;
        private readonly IMapper mapper;
        private readonly ILogger<TransactionCategoryService> logger;
        private readonly UserPackage userPackage;

        public TransactionCategoryService(
            ILogger<TransactionCategoryService> logger,
            IRepository<TransactionCategory> categoryRepository,
            IMapper mapper,
            UserPackage userPackage)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.userPackage = userPackage;
        }

        public async Task<TransactionCategoryModel> CreateCategoryAsync(TransactionCategoryModel categoryModel)
        {
            try
            {
                TransactionCategory transactionCategory = mapper.Map<TransactionCategory>(categoryModel);
                var result = await this.categoryRepository.AddAsync(transactionCategory);
                categoryModel.TransactionCategoryId = result.TransactionCategoryId;

                return categoryModel;
            }

            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create new category. UserId {this.userPackage.UserId}.", categoryModel);
            }

            return null;
        }

        public async Task<IEnumerable<TransactionCategoryModel>> GetAllCategoriesAsync()
        {
            IEnumerable<TransactionCategoryModel> result = new List<TransactionCategoryModel>();
            try
            {
                List<TransactionCategory> categories = await categoryRepository
                .GetAll()
                .ToListAsync();

                result = categories.Select(s => mapper.Map<TransactionCategoryModel>(s));

                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to gather categories. UserId {this.userPackage.UserId}.");
            }

            return null;
        }

        public async Task<TransactionCategoryModel> GetCategoryAsync(int id)
        {
            TransactionCategoryModel transactionCategoryModel = new TransactionCategoryModel();

            try
            {
                TransactionCategory transactionCategory = await this.categoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c => c.TransactionCategoryId == id);

                transactionCategoryModel = mapper.Map<TransactionCategoryModel>(transactionCategory);

                return transactionCategoryModel;
            }

            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get category with id {id}. UserId {this.userPackage.UserId}.");
            }

            return null;
        }

        public async Task RemoveCategoryAsync(int id)
        {
            try
            {
                TransactionCategory transactionCategory = await this.categoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c => c.TransactionCategoryId == id);

                transactionCategory.IsDeleted = true;
                transactionCategory.DeletedOnUtc = DateTime.UtcNow;
                await this.categoryRepository.RemoveAsync(transactionCategory);
            }

            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to remove category with id {id}. UserId {this.userPackage.UserId}.");
            }
        }

        public async Task<TransactionCategoryModel> UpdateCategoryAsync(TransactionCategoryModel categoryModel)
        {
            try
            {
                TransactionCategory transactionCategory = mapper.Map<TransactionCategory>(categoryModel);
                await this.categoryRepository.UpdateAsync(transactionCategory);

                return categoryModel;
            }

            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to update category. UserId {this.userPackage.UserId}.", categoryModel);
            }

            return null;
        }
    }
}
