using MoneySaver.Api.Models;

namespace MoneySaver.Api.Services.Utilities
{
    public class RequestsUtility
    {
        public static (int Page, int PageSize) CheckPageProperties(int page, int pageSize)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = Constants.ITEMS_PER_PAGE;
            }

            return (page, pageSize);
        }
    }
}
