using AutoMapper;
using MoneySaver.Api.Data;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
