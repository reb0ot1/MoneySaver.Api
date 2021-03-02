using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IReportsService
    {
        Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter);
    }
}
