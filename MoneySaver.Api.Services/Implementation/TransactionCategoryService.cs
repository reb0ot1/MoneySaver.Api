using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                //TODO Log Exception
                ;
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
                ;
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
               ;
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
                ;
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
                ;
            }

            return null;
        }
    }
}
