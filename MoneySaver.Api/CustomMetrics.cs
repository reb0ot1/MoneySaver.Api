using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace MoneySaver.Api
{
    public class CustomMetrics : IDisposable
    {
        private readonly Meter _meter;
        private readonly Counter<long> _httpRequestCounter;
        private readonly Histogram<double> _httpRequestDuration;

        public CustomMetrics()
        {
            // Create meter without IMeterFactory dependency
            _meter = new Meter("MoneySaver.Api.Metrics", "1.0.0");
            
            _httpRequestCounter = _meter.CreateCounter<long>(
                "moneysaver_http_requests_total",
                description: "Total number of HTTP requests"
            );

            _httpRequestDuration = _meter.CreateHistogram<double>(
                "moneysaver_http_request_duration_ms",
                unit: "ms",
                description: "Duration of HTTP requests in milliseconds"
            );
        }

        public void RecordHttpRequestDuration(string method, string path, int statusCode, long durationMs)
        {
            var tags = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("http.method", method),
                new KeyValuePair<string, object>("http.route", path),
                new KeyValuePair<string, object>("http.status_code", statusCode)
            };

            _httpRequestCounter.Add(1, tags);
            _httpRequestDuration.Record(durationMs, tags);
        }

        public void Dispose()
        {
            _meter?.Dispose();
        }
    }
}
