using IntegrationProject.Enums;
using IntegrationProject.Helpers;
using IntegrationProject.Results;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationProject.Tests.Helpers
{
    public class ImportResponseHelperTests
    {
        [Fact]
        public void GetImportResponse_AllSuccess_ReturnsOk()
        {
            var results = new List<ImportResult>
            {
                new() { StepName = "Products", Status = ImportStatus.Success },
                new() { StepName = "Inventory", Status = ImportStatus.Success },
                new() { StepName = "Prices", Status = ImportStatus.Success }
            };

            var result = ImportResponseHelper.GetImportResponse(results);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal("Import completed successfully.", ok.Value);
        }

        [Fact]
        public void GetImportResponse_WithApiError_ReturnsBadRequest()
        {
            var results = new List<ImportResult>
            {
                new() { StepName = "Products", Status = ImportStatus.Success },
                new() { StepName = "Inventory", Status = ImportStatus.ApiError, Message = "Failed to fetch" },
                new() { StepName = "Prices", Status = ImportStatus.Success }
            };

            var result = ImportResponseHelper.GetImportResponse(results);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Contains("[Inventory] Failed to fetch", badRequest.Value.ToString());
        }

        [Fact]
        public void GetImportResponse_WithNonApiErrors_ReturnsMultiStatus()
        {
            var results = new List<ImportResult>
            {
                new() { StepName = "Products", Status = ImportStatus.Success },
                new() { StepName = "Inventory", Status = ImportStatus.NoData, Message = "No inventory found" },
                new() { StepName = "Prices", Status = ImportStatus.DbError, Message = "Failed to insert" }
            };

            var result = ImportResponseHelper.GetImportResponse(results);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(207, objectResult.StatusCode);
            var msg = objectResult.Value?.ToString();
            Assert.Contains("[Inventory] No inventory found", msg);
            Assert.Contains("[Prices] Failed to insert", msg);
        }

        [Fact]
        public void GetImportResponse_EmptyList_ReturnsOk()
        {
            var results = new List<ImportResult>();

            var result = ImportResponseHelper.GetImportResponse(results);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Import completed successfully.", ok.Value);
        }
    }
}
