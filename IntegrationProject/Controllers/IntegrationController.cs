using IntegrationProject.Helpers;
using IntegrationProject.Results;
using IntegrationProject.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        /// Executes a full data integration process by downloading CSV files from external APIs,
        /// saving them locally, parsing their contents, applying necessary filters, and inserting the data into the local SQL database.
        /// 
        /// The endpoint handles the following import steps:
        /// - **Products**: Excludes products that are wires or do not ship within 24 hours.
        /// - **Inventory**: Includes only items with shipping time less than or equal to 24 hours.
        /// - **Prices**: Imports all available price data without filtering.
        /// 
        /// Each import step is validated individually. The response aggregates the status of each operation
        /// (e.g., success, API error, DB error, no data, unexpected error).
        /// 
        /// This endpoint is intended to be called manually or scheduled periodically to synchronize the local database with the latest external product, inventory, and pricing data.
        /// </summary>
        /// <returns>
        /// HTTP 200 OK with a structured summary of import results for each data type (products, inventory, prices).
        /// </returns>
        [HttpPost("import")]
        [SwaggerOperation(
            Summary = "Imports all external CSV data into the local SQL database.",
            Description = "Downloads CSV files for products, inventory, and prices. Each file is parsed, filtered and saved into the local database. Returns the status of each step in a structured response."
        )]
        [ProducesResponseType(typeof(List<ImportResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportDataToLocalDb()
        {
            var results = await _integrationService.ImportDataToLocalDb();

            return ImportResponseHelper.GetImportResponse(results);
        }
    }
}
