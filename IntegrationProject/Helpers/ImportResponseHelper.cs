using IntegrationProject.Enums;
using IntegrationProject.Results;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationProject.Helpers
{
    internal static class ImportResponseHelper
    {
        internal static IActionResult GetImportResponse(List<ImportResult> results)
        {
            if (results.All(r => r.Status == ImportStatus.Success))
            {
                return new OkObjectResult("Import completed successfully.");
            }

            var errors = results
                .Where(r => r.Status == ImportStatus.ApiError)
                .Select(r => $"- [{r.StepName}] {r.Message}")
                .ToList();

            if (errors.Any())
            {
                var errorResponse = "Import failed for the following steps:\n" + string.Join("\n", errors);
                
                return new BadRequestObjectResult(errorResponse);
            }

            var warnings = results
                .Select(r => $"- [{r.StepName}] {r.Message}")
                .ToList();

            var warningResponse = "Import partially completed with warnings:\n" + string.Join("\n", warnings);
            
            return new ObjectResult(warningResponse) { StatusCode = 207 };
        }
    }
}
