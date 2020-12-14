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

        public TransactionCategoryModel CreateCategory(TransactionCategoryModel categoryModel)
        {
            TransactionCategory transactionCategory = mapper.Map<TransactionCategory>(categoryModel);
            this.categoryRepository.AddAsync(transactionCategory);

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
