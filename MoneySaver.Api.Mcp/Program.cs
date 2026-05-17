using MoneySaver.Api.Mcp;
using MoneySaver.Api.Mcp.Models;
using MoneySaver.Api.Mcp.Models.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.Configure<UrlRoutesConfiguration>(builder.Configuration.GetSection(nameof(UrlRoutesConfiguration)));
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp =>
{
    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
    var apiKey = accessor.HttpContext?
        .Request.Headers["AuthK"].FirstOrDefault() ?? string.Empty;

    return new ApiKeyModel { ApiKey = apiKey };
});
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TransactionService>();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();
