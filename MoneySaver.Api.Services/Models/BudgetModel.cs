using MoneySaver.Api.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Services.Models
{
    public class BudgetModel
    {
        public int Id { get; set; }

        public BudgetType Type { get; set; }
    }
}
