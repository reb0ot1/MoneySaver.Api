using ModelContextProtocol.Server;
using MoneySaver.Api.Mcp.Models;
using System.ComponentModel;
using System.Text.Json;

namespace MoneySaver.Api.Mcp
{
    [McpServerToolType]
    public static class CategoryTools
    {
        [McpServerTool, Description("" +
            "Get a list of categories for a particular user" +
            "This will return a JSON format string as a result"
            )]
        public static async Task<string> GetCategories(CategoryService categoryService, IHttpContextAccessor httpContextAccessor)
        {
            //TODO: Add an http request to the MoneysaverAPI
            var result = await categoryService.GetCategoriesAsync();
            //var key = httpContextAccessor.HttpContext?
            //                .Request.Headers["AuthK"].FirstOrDefault() ?? "notfound";
            //var result = await categoryService.GetCategoriesAsync(new ApiKeyModel { ApiKey = key });
            return JsonSerializer.Serialize(result);
        }
    }
}
