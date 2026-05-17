using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MoneySaver.Api.Mcp.Models;
using System.Security.Claims;

namespace MoneySaver.Api.Mcp.Middlewares
{
    public class GatherApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GatherApiKeyMiddleware> _logger;

        public GatherApiKeyMiddleware(RequestDelegate _next, ILogger<GatherApiKeyMiddleware> logger )
        {
            this._next = _next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, ApiKeyModel apiKeyModel)
        {
            httpContext.Request.Headers.TryGetValue("AuthK", out StringValues apiKey);
            
            if (StringValues.IsNullOrEmpty(apiKey))
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)StatusCodes.Status401Unauthorized;
                return;
            }
            
            apiKeyModel.ApiKey = apiKey.ToString();
            this._logger.LogWarning("Hashcode ->>>"+apiKeyModel.GetHashCode().ToString());
            await this._next(httpContext);
        }
    }
}
