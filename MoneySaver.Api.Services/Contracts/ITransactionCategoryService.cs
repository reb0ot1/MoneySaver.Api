using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface ITransactionCategoryService
    {
        Task<IEnumerable<TransactionCategoryModel>> GetAllCategoriesAsync();
        TransactionCategoryModel GetCategory(int id);
        TransactionCategoryModel UpdateCategory(TransactionCategoryModel categoryModel);
        TransactionCategoryModel CreateCategory(TransactionCategoryModel categoryModel);
        void RemoveCategory(int id);
    }
}
