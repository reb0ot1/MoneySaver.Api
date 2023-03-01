using MoneySaver.Api.Models;
using System;

namespace MoneySaver.Api.Services.Utilities
{
    public static class DateUtility
    {
        public static (DateTime Start, DateTime End) GetStartEndDateByMonthInterval(DateTime start, DateTime end)
        {
            var daysInMonth = DateTime.DaysInMonth(end.Year, end.Month);
            var dateEndMonth = new DateTime(end.Year, end.Month, daysInMonth).AddDays(1).AddTicks(-1);
            var dateStartMonth = new DateTime(start.Year, start.Month, 1);

            return (dateStartMonth, dateEndMonth);
        }

        public static (DateTime Start, DateTime End) GetPeriodByBudgetType(Models.BudgetType budgetType, DateTime date)
        {
            DateTime startDate;
            DateTime endDate;

            switch (budgetType)
            {
                case Models.BudgetType.Weekly:
                    startDate = date;
                    endDate = date;
                    break;
                case Models.BudgetType.Monthly:
                    var result = GetStartEndDateByMonthInterval(date, date);
                    endDate = result.End;
                    startDate = result.Start;
                    break;
                case Models.BudgetType.Yearly:
                    endDate = new DateTime(date.Year, 12, 31);
                    startDate = new DateTime(date.Year, 01, 01);
                    break;

                default:
                    startDate = date;
                    endDate = date;
                    break;
            }

            return (startDate, endDate);
        }

        public static (DateTime Start, DateTime End) GetBudgetTypeNextDate(BudgetType budgetType, DateTime date)
        {
            DateTime? newDate = null;
            switch (budgetType)
            {
                case BudgetType.Weekly:
                    newDate = date.AddDays(7);
                    break;
                case BudgetType.Monthly:
                    newDate = date.AddMonths(1);
                    break;
                case BudgetType.Yearly:
                    newDate = date.AddYears(1);
                    break;
            }

            if (newDate == null)
            {
                throw new Exception($"No date created when parameters budgetType {budgetType} and date {date} are used. Method [{nameof(DateUtility.GetBudgetTypeNextDate)}]");
            }

            return GetPeriodByBudgetType(budgetType, (DateTime)newDate);
        }
    }
}
