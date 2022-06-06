using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Models.Response
{
    public class PageModel<T>
    {
        public int TotalCount { get; set; }

        public IEnumerable<T> Result { get; set; }
    }
}
