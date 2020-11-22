using AutoMapper;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoneySaver.Api.Services.Implementation
{
    public class BudgetService : IBudgetService
    {
        private IRepository<Budget> budgetRepository;
        private IMapper mapper;

        public BudgetService(IRepository<Budget> budgetRepository, IMapper mapper)
        {
            this.budgetRepository = budgetRepository;
            this.mapper = mapper;
        }

        public BudgetModel CreateBudget(BudgetModel budgetModel)
        {
            Budget budget = mapper.Map<Budget>(budgetModel);
            this.budgetRepository.AddAsync(budget);

            return budgetModel;
        }

        public List<BudgetModel> GetAllBudgets()
        {
            List<BudgetModel> budgetModels = budgetRepository.GetAll().Select(m => mapper.Map<BudgetModel>(m)).ToList();

            return budgetModels;
        }

        public BudgetModel GetBudget(int id)
        {
            Budget budget = this.budgetRepository.GetAll().FirstOrDefault(b => b.Id == id);
            BudgetModel budgetModel = mapper.Map<BudgetModel>(budget);

            return budgetModel;
        }

        public void RemoveBudget(int id)
        {
            Budget budget = this.budgetRepository.GetAll().FirstOrDefault(b => b.Id == id);
            budget.IsDeleted = true;
            budget.DeletedOnUtc = DateTime.UtcNow;
            this.budgetRepository.RemoveAsync(budget);
        }

        public BudgetModel UpdateBudget(BudgetModel budgetModel)
        {
            Budget budget = mapper.Map<Budget>(budgetModel);
            this.budgetRepository.UpdateAsync(budget);

            return budgetModel;
        }
    }
}
