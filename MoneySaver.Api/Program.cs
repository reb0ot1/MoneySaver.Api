using HealthChecks.UI.Client;
using MoneySaver.Api;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Middlewares;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Implementation;
using MoneySaver.System.Infrastructure;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddWebService<MoneySaverApiContext>(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionCategoryService, TransactionCategoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<UserPackage>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IAppConfigurationService, AppConfigurationService>();
builder.Services.AddScoped<IDateProvider, DateProvider>();
builder.Services.AddSingleton<CustomMetrics>();
builder.Services.AddCors(options =>
{
    //TODO: Change the CORS policy
    options.AddPolicy("Open", b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Moneysaver.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation()
            .AddOtlpExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddMeter("MoneySaver.Api.Metrics")
            .AddPrometheusExporter();
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("Open");
app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseMiddleware<MetricsMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseUserPackageMiddleware();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
