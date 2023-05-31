using MoneySaver.Api.Models;
using MoneySaver.System.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface ITransactionCategoryService
    {
        Task<Result<IEnumerable<TransactionCategoryModel>>> GetAllCategoriesAsync();
        Task<Result<TransactionCategoryModel>> GetCategoryAsync(int id);
        Task<Result<TransactionCategoryModel>> UpdateCategoryAsync(TransactionCategoryModel categoryModel);
        Task<Result<TransactionCategoryModel>> CreateCategoryAsync(TransactionCategoryModel categoryModel);
        Task <Result<bool>> RemoveCategoryAsync(int id);
    }
}
