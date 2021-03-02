using AutoMapper;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;

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
        }
    }
}
