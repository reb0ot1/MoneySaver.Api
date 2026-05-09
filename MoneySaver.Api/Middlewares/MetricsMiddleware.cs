using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MoneySaver.Api.Middlewares
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CustomMetrics _customMetrics;

        public MetricsMiddleware(RequestDelegate next, CustomMetrics customMetrics)
        {
            _next = next;
            _customMetrics = customMetrics;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip metrics for health and metrics endpoints
            if (context.Request.Path.StartsWithSegments("/health") || 
                context.Request.Path.StartsWithSegments("/metrics"))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                _customMetrics.RecordHttpRequestDuration(
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }
    }
}
