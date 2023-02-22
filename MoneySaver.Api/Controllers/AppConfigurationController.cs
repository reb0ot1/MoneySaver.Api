using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneySaver.Api.Services.Contracts;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppConfigurationController : Controller
    {
        private IAppConfigurationService configService;

        public AppConfigurationController(IAppConfigurationService configService)
        {
            this.configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppConfig()
        {
            var result = await this.configService.GetUserConfiguration();
            if (!result.Succeeded)
            {
                return this.BadRequest(result.Errors);
            }
            return this.Ok(result.Data);
        }

        [HttpPost("setuserconfig")]
        public async Task<IActionResult> GetUserConfig()
        {
            var result = await this.configService.SetUserConfiguration();

            if (!result.Succeeded)
            {
                return this.BadRequest(result.Errors);
            }

            return this.Ok(result.Data);
        }
    }
}
