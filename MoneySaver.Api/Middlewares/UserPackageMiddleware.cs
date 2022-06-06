using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneySaver.Api.Middlewares
{
    public class UserPackageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserPackageMiddleware> logger;

        public UserPackageMiddleware(RequestDelegate _next, ILogger<UserPackageMiddleware> logger)
        {
            this._next = _next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, UserPackage userPackage)
        {
            if (httpContext.User.Identity is ClaimsIdentity identity)
            {
                var userId = identity.FindFirst("sub")?.Value;
                if (userId == null)
                {
                    userId = identity.FindFirst("nameid")?.Value;
                }

                userPackage.UserId = userId;
            }

            await this._next(httpContext);
        }
    }
}
