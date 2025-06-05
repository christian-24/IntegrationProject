using FluentAssertions;
using IntegrationProject.Helpers;
using IntegrationProject.Mappers;
using IntegrationProject.Models;

namespace IntegrationProject.Tests.Helpers
{
    public class ParserHelperTests
    {
        [Fact]
        public void ParseCsv_ShouldParseValidInventoryData()
        {
            var csv = """
            product_id,unit,qty,manufacturer_name,shipping,shipping_cost
            123,szt.,5.5,ACME,24h,12.34
            """;

            using var reader = new StringReader(csv);
            using var stream = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv)));

            var records = ParserHelper.ParseCsv<InventoryModel, InventoryMapper>(
                streamReader: stream,
                delimeter: ",",
                hasHeader: true,
                shouldSkipEmptyLine: true
            );

            records.Should().HaveCount(1);
            var model = records[0];
            model.ProductId.Should().Be(123);
            model.Unit.Should().Be("szt.");
            model.Qty.Should().Be(5.5m);
            model.Manufacturer.Should().Be("ACME");
            model.IsShippingIn24Hours.Should().BeTrue();
            model.ShippingCost.Should().Be(12.34m);
        }

        [Fact]
        public void ParseCsv_ShouldSkipEmptyLines_WhenFlagEnabled()
        {
            var csv = """
            product_id,unit,qty,manufacturer_name,shipping,shipping_cost

            123,szt.,1.5,ACME,24h,10.00
            __empty_line__
            124,szt.,2.5,ACME,48h,12.00
            """;

            using var stream = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv)));

            var result = ParserHelper.ParseCsv<InventoryModel, InventoryMapper>(
                stream,
                ",",
                true,
                shouldSkipEmptyLine: true
            );

            result.Should().HaveCount(2);
            result.Select(x => x.ProductId).Should().BeEquivalentTo(new[] { 123, 124 });
        }
    }
}
