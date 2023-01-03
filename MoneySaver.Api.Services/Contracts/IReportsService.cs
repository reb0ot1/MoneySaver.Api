using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IReportsService
    {
        Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter);

        Task<LineChartData> GetExpensesByPeriod(FilterModel filter);

        Task<LineChartData> GetExpensesForPeriodByCategoriesAsync(FilterModel filter);
    }
}
