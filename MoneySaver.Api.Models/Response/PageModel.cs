using System.Collections.Generic;

namespace MoneySaver.Api.Models.Response
{
    public class PageModel<T>
    {
        public PageModel()
        {
            this.Result = new List<T>();
        }

        public int TotalCount { get; set; } = 0;

        public IEnumerable<T> Result { get; set; }
    }
}
