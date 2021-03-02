using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Models.Filters
{
    public class FilterModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
