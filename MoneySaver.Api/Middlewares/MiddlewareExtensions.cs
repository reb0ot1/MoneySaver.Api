using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseUserPackageMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserPackageMiddleware>();
        }
    }
}
