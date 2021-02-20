using MoneySaver.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface ITransactionCategoryService
    {
        Task<IEnumerable<TransactionCategoryModel>> GetAllCategoriesAsync();
        Task<TransactionCategoryModel> GetCategoryAsync(int id);
        Task<TransactionCategoryModel> UpdateCategoryAsync(TransactionCategoryModel categoryModel);
        Task<TransactionCategoryModel> CreateCategoryAsync(TransactionCategoryModel categoryModel);
        Task RemoveCategoryAsync(int id);
    }
}
