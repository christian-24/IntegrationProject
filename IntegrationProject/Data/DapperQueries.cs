namespace IntegrationProject.Data
{
    internal class DapperQueries
    {
        internal class InsertQueries
        {
            public const string InsertProductsSql = @"
                INSERT INTO Products (
                    Id, Sku, Name, Ean, ProducerName, Category,
                    IsWire, Shipping, Available, IsVendor, DefaultImage)
                VALUES (
                    @Id, @Sku, @Name, @Ean, @ProducerName, @Category,
                    @IsWire, @Shipping, @Available, @IsVendor, @DefaultImage)";

            public const string InsertInventorySql = @"
                INSERT INTO Inventory (
                    ProductId, Unit, Qty, Manufacturer, ShippingCost)
                VALUES (
                    @ProductId, @Unit, @Qty, @Manufacturer, @ShippingCost)";

            public const string InsertPricesSql = @"
                INSERT INTO Prices (
                    Sku, NettPrice, NettPriceDiscount, NettPriceLogisticDiscount)
                VALUES (
                    @Sku, @NettPrice, @NettPriceDiscount, @NettPriceLogisticDiscount)";
        }
    }
}
