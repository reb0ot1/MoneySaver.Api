using MoneySaver.Api.Models.Response;
using MoneySaver.System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IAppConfigurationService
    {
        Task<Result<bool>> SetUserConfiguration();

        Task<Result<AppConfigResponseModel>> GetUserConfiguration();
    }
}
