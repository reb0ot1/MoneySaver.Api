using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Middlewares;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Implementation;
using MoneySaver.System.Infrastructure;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace MoneySaver.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(this.Configuration)
              .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton(Configuration.GetSection(nameof(Authority)).Get<Authority>());
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddLogging(logging =>
            {
                logging.AddSerilog(dispose:true);
            });

            services.AddWebService<MoneySaverApiContext>(Configuration);
            //services.AddHealthChecks();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ITransactionCategoryService, TransactionCategoryService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<UserPackage>();
            services.AddScoped<IReportsService, ReportsService>();
            services.AddScoped<IAppConfigurationService, AppConfigurationService>();
            services.AddScoped<IDateProvider, DateProvider>();
            services.AddCors(options =>
            {
                //TODO: Change the CORS policy
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddControllers();

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.Authority = Configuration["Authority:Url"];
            //        options.Audience = Configuration["Authority:Audiance"];
            //    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("Open");
            app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { 
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseUserPackageMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
