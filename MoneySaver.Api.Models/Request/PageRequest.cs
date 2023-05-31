using MoneySaver.Api.Models.Filters;

namespace MoneySaver.Api.Models.Request
{
    public class PageRequest
    {
        public int ItemsToSkip { get; set; }

        public int ItemsPerPage { get; set; }

        public FilterModel Filter { get; set; }
    }
}
