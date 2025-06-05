using IntegrationProject.Dtos;
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

        /// <summary>
        /// Returns product details based on the provided SKU.
        /// 
        /// This includes:
        /// - Product name, EAN, category and image,
        /// - Inventory quantity and unit,
        /// - Net price based on discount rules (logistic or standard),
        /// - Shipping cost.
        /// 
        /// The result is aggregated from the Products, Inventory, and Prices tables.
        /// Pricing logic:
        /// - If unit is "szt." and logistic discount > 0 → use it.
        /// - Else if standard discount > 0 → use it.
        /// - Else use base net price.
        /// </summary>
        /// <param name="sku">The unique product SKU identifier (from the warehouse).</param>
        /// <returns>
        /// HTTP 200 OK with product details, or 404 Not Found if the SKU does not exist.
        /// </returns>
        [HttpGet("{sku}")]
        [SwaggerOperation(
            Summary = "Gets product details by SKU",
            Description = "Returns detailed product information including name, quantity, price and shipping cost by searching in Products, Inventory and Prices tables."
        )]
        [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductBySku(string sku)
        {
            var result = await _integrationService.GetProductDetailBaseOnSku(sku);

            if (result == null)
            {
                return NotFound($"No product found for SKU '{sku}'");
            }

            return Ok(result);
        }
    }
}
