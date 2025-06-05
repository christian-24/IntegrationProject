namespace IntegrationProject.Data
{
    internal class DapperQueries
    {
        internal class InsertQueries
        {
            public const string InsertProductsSql = @"
                INSERT INTO Integration.dbo.Products (
                    Id, Sku, Name, Ean, ProducerName, Category,
                    IsWire, Shipping, Available, IsVendor, DefaultImage)
                VALUES (
                    @Id, @Sku, @Name, @Ean, @ProducerName, @Category,
                    @IsWire, @Shipping, @Available, @IsVendor, @DefaultImage)";

            public const string InsertInventorySql = @"
                INSERT INTO Integration.dbo.Inventory (
                    ProductId, Unit, Qty, Manufacturer, ShippingCost)
                VALUES (
                    @ProductId, @Unit, @Qty, @Manufacturer, @ShippingCost)";

            public const string InsertPricesSql = @"
                INSERT INTO Integration.dbo.Prices (
                    Sku, NettPrice, NettPriceDiscount, NettPriceLogisticDiscount)
                VALUES (
                    @Sku, @NettPrice, @NettPriceDiscount, @NettPriceLogisticDiscount)";
        }

        internal class SelectQueries
        {
            internal const string GetProductDetailBaseOnSku = @"
                SELECT
                    p.Name,
                    p.Ean,
                    p.Category,
                    p.DefaultImage,
                    i.Qty,
                    i.Unit,
                    CASE
                        WHEN i.Unit = 'szt.' AND ISNULL(pc.NettPriceLogisticDiscount, 0) > 0 THEN pc.NettPriceLogisticDiscount
                        WHEN ISNULL(pc.NettPriceDiscount, 0) > 0 THEN pc.NettPriceDiscount
                        ELSE pc.NettPrice
                    END AS Price,
                    i.ShippingCost
                FROM Integration.dbo.Products p
                JOIN Integration.dbo.Inventory i ON p.Id = i.ProductId
                JOIN Integration.dbo.Prices pc ON p.Sku = pc.Sku
                WHERE p.Sku = @Sku";
        }
    }
}
