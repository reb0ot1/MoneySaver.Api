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
            CreateMap<Transaction, TransactionModel>();
            CreateMap<TransactionModel, Transaction>();
            CreateMap<TransactionCategory, TransactionCategoryModel>();
            CreateMap<TransactionCategoryModel, TransactionCategory>();
            CreateMap<Budget, BudgetModel>();
            CreateMap<BudgetModel, Budget>();
        }
    }
}
