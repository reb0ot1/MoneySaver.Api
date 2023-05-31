using MoneySaver.Api.Services.Contracts;
using System;

namespace MoneySaver.Api.Services.Implementation
{
    public class DateProvider : IDateProvider
    {
        public DateTime GetDateTimeNow()
         => DateTime.UtcNow;
    }
}
