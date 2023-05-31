using System;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IDateProvider
    {
        DateTime GetDateTimeNow();
    }
}
