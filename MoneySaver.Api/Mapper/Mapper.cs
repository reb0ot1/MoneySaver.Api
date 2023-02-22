using AutoMapper;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Response;

namespace MoneySaver.Api
{
    public class Mapper : Profile
    {
        public Mapper() 
        {
            CreateMap<Transaction, TransactionModel>().ReverseMap();
            CreateMap<TransactionCategory, TransactionCategoryModel>().ReverseMap();
            CreateMap<Budget, BudgetModel>().ReverseMap();
            CreateMap<BudgetItem, BudgetItemModel>().ReverseMap();
            CreateMap<Budget, BudgetResponseModel>();
        }
    }
}
