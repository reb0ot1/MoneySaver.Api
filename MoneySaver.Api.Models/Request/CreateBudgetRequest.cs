using System;

namespace MoneySaver.Api.Models.Request
{
    //TODO: Use DatMember
    public class CreateBudgetRequest
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        //TODO: Use enum option name instead enum option value
        public BudgetType BudgetType { get; set; }

        public bool CopyPrevItems { get; set; }
    }
}
