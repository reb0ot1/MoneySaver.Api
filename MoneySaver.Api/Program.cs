using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MoneySaver.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var config = host.Services.GetRequiredService<IConfiguration>();
            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json",
                    optional: true,
                    reloadOnChange: true);
                })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}

// using MoneySaver.Identity.Data;
// using MoneySaver.System.Infrastructure;
// using MoneySaver.System.Services;
// using MoneySaver.Identity.Infrastructure;
// using MoneySaver.Identity.Services.Identity;
// using HealthChecks.UI.Client;
// using Serilog;
// using MoneySaver.Identity.Models.Configuration;
//
// var builder = WebApplication.CreateBuilder(args);
// Log.Logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(builder.Configuration)
//     .CreateLogger();

// Add services to the container.
// builder.Services.AddWebService<IdentityDbContext>(builder.Configuration);
// builder.Services.Configure<UrlRoutesConfiguration>(builder.Configuration.GetSection(nameof(UrlRoutesConfiguration)));
// builder.Services.AddLogging(logging =>
// {
//     logging.AddSerilog(dispose: true);
// });
// builder.Services.AddHttpClient();
// builder.Services.AddUserStorage();
// builder.Services.AddTransient<IDataSeeder, IdentityDataSeeder>();
// builder.Services.AddTransient<IIdentityService, IdentityService>()
//     .AddTransient<ITokenGeneratorService, TokenGeneratorService>();
//
// builder.Services.AddHealthChecks();
//
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Host.UseSerilog();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// //if (app.Environment.IsDevelopment())
// //{
// //    app.UseSwagger();
// //    app.UseSwaggerUI();
// //}
//
// app.MapHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { 
//     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
// });
//
// app.UseWebService(app.Environment)
//     .Initialize();
//
// app.Run();


