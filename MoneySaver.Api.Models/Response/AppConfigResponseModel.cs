using MoneySaver.Api.Models.Enums;

namespace MoneySaver.Api.Models.Response
{
    public class AppConfigResponseModel
    {
        public BudgetType BudgetType { get; set; }

        public CurrencyType Currency { get; set; }
    }
}
