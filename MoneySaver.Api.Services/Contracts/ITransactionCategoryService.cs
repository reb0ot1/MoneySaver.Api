using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Services.Contracts
{
    public interface ITransactionCategoryService
    {
        List<TransactionCategoryModel> GetAllCategories();
        TransactionCategoryModel GetCategory(int id);
        TransactionCategoryModel UpdateCategory(TransactionCategoryModel categoryModel);
        TransactionCategoryModel CreateCategory(TransactionCategoryModel categoryModel);
        void RemoveCategory(int id);
    }
}
