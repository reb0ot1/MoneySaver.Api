using Microsoft.Extensions.Options;
using MoneySaver.Api.Mcp.Models;
using MoneySaver.Api.Mcp.Models.Configurations;

namespace MoneySaver.Api.Mcp
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiKeyModel _apiKeyModel;

        private readonly UrlRoutesConfiguration _urlRoutesConfig;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            IHttpClientFactory httpClientFactory, 
            ApiKeyModel apiKeyModel, 
            IOptions<UrlRoutesConfiguration> urlRoutesConfig,
            ILogger<CategoryService> logger)
        {
            this._httpClient = httpClientFactory.CreateClient();
            this._apiKeyModel = apiKeyModel;
            this._urlRoutesConfig = urlRoutesConfig.Value;
            _logger = logger;
        }

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_urlRoutesConfig.ApiUrl}/api/category"));
            request.Headers.Add("Accept", "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", this._apiKeyModel.ApiKey);
            var response = await this._httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            { 
                var result = await response.Content.ReadFromJsonAsync<List<CategoryModel>>();
                if (result == null)
                {
                    return new List<CategoryModel>();
                }

                return result;
            }

            return new List<CategoryModel>();
        }

        public async Task<List<CategoryModel>> GetCategoriesAsync(ApiKeyModel _apiKeyModel)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_urlRoutesConfig.ApiUrl}/api/category"));
            request.Headers.Add("Accept", "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", _apiKeyModel.ApiKey);
            var response = await this._httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<CategoryModel>>();
                if (result == null)
                {
                    return new List<CategoryModel>();
                }

                return result;
            }

            return new List<CategoryModel>();
        }
    }
}
