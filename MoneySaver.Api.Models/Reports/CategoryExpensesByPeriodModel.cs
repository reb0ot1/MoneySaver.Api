using MoneySaver.Api.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Models.Reports
{
    public class CategoryExpensesByPeriodModel
    {
        public string DateTimeAsString { get; set; }

        public DateTime Date { get; set; }

        public IdAndNameModel[] Categories { get; set; }
    }
}
