using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Response;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.System.Services;
using System;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly ILogger<AppConfigurationService> _logger;
        private readonly IRepository<AppConfiguration> _appConfigurationRepository;
        private readonly UserPackage _userPackage;


        public AppConfigurationService(
            ILogger<AppConfigurationService> logger,
            IRepository<AppConfiguration> appConfigurationRepo,
            UserPackage userPack
            )
        {
            this._logger = logger;
            this._appConfigurationRepository = appConfigurationRepo;
            this._userPackage = userPack;
        }

        public async Task<Result<AppConfigResponseModel>> GetUserConfiguration()
        {
            var config = await this._appConfigurationRepository
                .GetAll()
                .FirstOrDefaultAsync();

            return new AppConfigResponseModel
            {
                BudgetType = config.BudgetType,
                Currency = config.Currency
            };
        }

        public async Task<Result<bool>> SetUserConfiguration()
        {
            try
            {
                //TODO: Try to set in the table that userid property is unique 
                var config = await this._appConfigurationRepository
                .GetAll()
                .FirstOrDefaultAsync();

                if (config == null)
                {
                    await this._appConfigurationRepository
                        .AddAsync(new AppConfiguration
                        {
                            Currency = Models.Enums.CurrencyType.BGN,
                            UserId = this._userPackage.UserId
                        });
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Failed to create configuration for user with id [{0}]", this._userPackage.UserId);
                return "Failed to create configuration";
            }

            return true;
        }
    }
}
