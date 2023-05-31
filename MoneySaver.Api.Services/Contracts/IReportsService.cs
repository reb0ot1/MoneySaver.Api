using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IReportsService
    {
        Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter);

        Task<LineChartData> GetExpensesByPeriod(FilterModel filter);

        Task<LineChartData> GetExpensesForPeriodByCategoriesAsync(FilterModel filter);

        Task<IEnumerable<IdValue<double?>>> SpentAmountPerCategorieAsync(PageRequest pageRequest);
    }
}
