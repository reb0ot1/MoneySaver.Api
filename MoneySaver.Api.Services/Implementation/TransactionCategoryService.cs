using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.System.Services;
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

        public async Task<Result<TransactionCategoryModel>> CreateCategoryAsync(TransactionCategoryModel categoryModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryModel.Name))
                {
                    return "Missing category name.";
                }

                var categoryDb = await this.categoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.Name == categoryModel.Name);

                if (categoryDb is not null)
                {
                    return $"Category with name [{categoryModel.Name}] already exist.";
                }

                if (categoryModel.ParentId is not null)
                {
                    var parrentCategoryExists = await this.categoryRepository
                                                            .GetAll()
                                                            .AnyAsync(e => e.TransactionCategoryId == categoryModel.ParentId);
                    if (!parrentCategoryExists)
                    {
                        return $"Parrent category with id [{categoryModel.ParentId}] does not exist.";
                    }
                }

                TransactionCategory transactionCategory = mapper.Map<TransactionCategory>(categoryModel);
                var result = await this.categoryRepository.AddAsync(transactionCategory);
                categoryModel.TransactionCategoryId = result.TransactionCategoryId;

                return categoryModel;
            }

            catch (Exception ex)
            {
                var message = $"Failed to create new category. UserId {this.userPackage.UserId}.";
                this.logger.LogError(ex, message, categoryModel);

                return message;
            }
        }

        public async Task<Result<IEnumerable<TransactionCategoryModel>>> GetAllCategoriesAsync()
        {
            try
            { 
                List<TransactionCategoryModel> categories = await categoryRepository
                .GetAll()
                .Select(e => new TransactionCategoryModel { 
                    TransactionCategoryId = e.TransactionCategoryId,
                    Name = e.Name,
                    ParentId = e.ParentId
                })
                .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                var message = $"Failed to gather categories. UserId {this.userPackage.UserId}.";
                this.logger.LogError(ex, message);

                return message;
            }
        }

        public async Task<Result<TransactionCategoryModel>> GetCategoryAsync(int id)
        {
            try
            {
                TransactionCategory transactionCategory = await this.categoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c => c.TransactionCategoryId == id);

                if (transactionCategory is null)
                {
                    return $"Category with id [{id}] does not exist.";
                }

                return mapper.Map<TransactionCategoryModel>(transactionCategory);
            }

            catch (Exception ex)
            {
                var message = $"Failed to get category with id {id}. UserId {this.userPackage.UserId}.";
                this.logger.LogError(ex, message);

                return message;
            }
        }

        [Obsolete]
        public async Task<Result<bool>> RemoveCategoryAsync(int id)
        {
            try
            {
                TransactionCategory transactionCategory = await this.categoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(c => c.TransactionCategoryId == id);

                transactionCategory.IsDeleted = true;
                transactionCategory.DeletedOnUtc = DateTime.UtcNow;
                await this.categoryRepository.SetAsDeletedAsync(transactionCategory);

                return true;
            }

            catch (Exception ex)
            {
                var message = $"Failed to remove category with id {id}. UserId {this.userPackage.UserId}.";
                this.logger.LogError(ex, message);

                return message;
            }
        }

        public async Task<Result<TransactionCategoryModel>> UpdateCategoryAsync(TransactionCategoryModel categoryModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryModel.Name))
                {
                    return "Missing category name.";
                }

                if (categoryModel.TransactionCategoryId is null)
                {
                    return "Missing transaction category id.";
                }

                var categoryDb = await this.categoryRepository
                    .GetAll()
                    .FirstOrDefaultAsync(e => e.TransactionCategoryId == categoryModel.TransactionCategoryId);

                if (categoryDb is null)
                {
                    return $"Category with id [{categoryModel.TransactionCategoryId}] does not exist.";
                }

                if (categoryModel.ParentId is not null)
                {
                    var parrentCategoryExists = await this.categoryRepository
                        .GetAll()
                        .AnyAsync(e => e.TransactionCategoryId == categoryModel.ParentId);

                    if (!parrentCategoryExists)
                    {
                        return $"Parrent category with id [{categoryModel.ParentId}] does not exist.";
                    }
                }

                categoryDb.Name = categoryModel.Name;
                categoryDb.ParentId = categoryModel.ParentId;
                await this.categoryRepository.UpdateAsync(categoryDb);

                return categoryModel;
            }

            catch (Exception ex)
            {
                var message = $"Failed to update category. UserId {this.userPackage.UserId}.";
                this.logger.LogError(ex, message, categoryModel);
                
                return message;
            }
        }
    }
}
