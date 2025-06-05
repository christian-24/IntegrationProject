using IntegrationProject.Helpers;
using IntegrationProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntegrationController : ControllerBase
    {

        private readonly ILogger<IntegrationController> _logger;
        private readonly IntegrationService             _integrationService;

        public IntegrationController(ILogger<IntegrationController> logger, IntegrationService integrationService)
        {
            _logger             = logger;
            _integrationService = integrationService;
        }

        /// <summary>
        /// Endpoint do pobierania plików CSV i zapisu danych do bazy SQL.
        /// </summary>
        [HttpPost("import")]
        public async Task<IActionResult> ImportDataToLocalDb()
        {
            var results = await _integrationService.ImportDataToLocalDb();

            return ImportResponseHelper.GetImportResponse(results);
        }
    }
}
