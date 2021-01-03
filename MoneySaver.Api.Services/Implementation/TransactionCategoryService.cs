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
                await this.categoryRepository.AddAsync(transactionCategory);
            }

            catch (Exception ex)
            {
                ;
            }

            return categoryModel;
        }

        public async Task<IEnumerable<TransactionCategoryModel>> GetAllCategoriesAsync()
        {
            IEnumerable<TransactionCategoryModel> result = new List<TransactionCategoryModel>();
            try
            {
                List<TransactionCategory> categories = await categoryRepository
                .GetAll()
                .ToListAsync();

                var parentTransactionCategoryModels = categories
                    .Where(w => w.ParentId == null)
                    .Select(s => new TransactionCategoryModel
                    {
                        TransactionCategoryId = s.TransactionCategoryId,
                        Name = s.Name
                    })
                    .ToList();

                foreach (var parentCategory in parentTransactionCategoryModels)
                {
                    var children = categories.Where(w => w.ParentId == parentCategory.TransactionCategoryId);
                    if (children.Any())
                    {
                        parentCategory.Children = children
                            .Select(s => new TransactionCategoryModel
                            {
                                Name = s.Name,
                                TransactionCategoryId = s.TransactionCategoryId
                            });
                    }
                }

                result = parentTransactionCategoryModels;
            }
            catch (Exception ex)
            {
                ;
            }
            
            return result;
        }

        public async Task<TransactionCategoryModel> GetCategoryAsync(int id)
        {
            TransactionCategoryModel transactionCategoryModel = new TransactionCategoryModel();

            try
            {
                TransactionCategory transactionCategory = await this.categoryRepository.GetAll().FirstOrDefaultAsync(c => c.TransactionCategoryId == id);
                transactionCategoryModel = mapper.Map<TransactionCategoryModel>(transactionCategory);
            }

            catch (Exception ex)
            {
                ;
            }

            return transactionCategoryModel;
        }

        public async void RemoveCategoryAsync(int id)
        {
            try
            {
                TransactionCategory transactionCategory = await this.categoryRepository.GetAll().FirstOrDefaultAsync(c => c.TransactionCategoryId == id);
                transactionCategory.IsDeleted = true;
                transactionCategory.DeletedOnUtc = DateTime.UtcNow;
                this.categoryRepository.RemoveAsync(transactionCategory);
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
            }

            catch (Exception ex)
            {
                ;
            }

            return categoryModel;
        }
    }
}
