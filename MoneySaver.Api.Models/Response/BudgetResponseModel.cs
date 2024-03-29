﻿using System;

namespace MoneySaver.Api.Models.Response
{
    public class BudgetResponseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        //TODO: Use enum option name instead enum option value
        public BudgetType BudgetType { get; set; }

        public bool IsInUse { get; set; }
    }
}
