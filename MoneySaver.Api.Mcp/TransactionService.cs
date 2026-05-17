using Microsoft.Extensions.Options;
using MoneySaver.Api.Mcp.Models;
using MoneySaver.Api.Mcp.Models.Configurations;
using System.Text;
using System.Text.Json;

namespace MoneySaver.Api.Mcp
{
    public class TransactionService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiKeyModel _apiKeyModel;
        private readonly CategoryService _categoryService;
        private readonly UrlRoutesConfiguration _urlRoutesConfig;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            IHttpClientFactory httpClientFactory,
            ApiKeyModel apiKeyModel,
            IOptions<UrlRoutesConfiguration> urlRoutesConfig,
            ILogger<TransactionService> logger,
            CategoryService categoryService)
        {
            this._httpClient = httpClientFactory.CreateClient();
            this._apiKeyModel = apiKeyModel;
            this._urlRoutesConfig = urlRoutesConfig.Value;
            _logger = logger;
            _categoryService = categoryService;
        }

        public async Task<TransactionModel> CreateTransaction(
            string categoryName, 
            string dateTime, 
            double amount, 
            string additionalNote
            )
        {
            DateTime.TryParse(dateTime, out DateTime transactionDateTime);

            //We do not need all categories, we need only one to be found by name
            var allCategories = await this._categoryService.GetCategoriesAsync();

            var searchCategory = allCategories.FirstOrDefault(e => e.Name.ToLower() == categoryName.ToLower());
            if (searchCategory == null)
            {
                //TODO: Should be changed to receive errors
                return null;
            }

            var transactionToSend = new TransactionModel
            {
                Id = Guid.NewGuid().ToString(),
                AdditionalNote = additionalNote,
                Amount = amount,
                TransactionCategoryId = searchCategory.TransactionCategoryId,
                TransactionDate = transactionDateTime
            };

            this._logger.LogInformation("Object created");

            //TODO: Add env variable for url or use IOptions
            var request = new HttpRequestMessage(
                HttpMethod.Post, new Uri($"{_urlRoutesConfig.ApiUrl}/api/transaction"));
            request.Headers.Add("Accept", "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", this._apiKeyModel.ApiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(transactionToSend),
                Encoding.UTF8, "application/json"
                );

            var response = await this._httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TransactionModel>();
                return result;
            }

            //TODO: Should be changed to receive errors
            return null;
        }
    }
}
     
