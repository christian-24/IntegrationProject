using CsvHelper.Configuration;
using IntegrationProject.Extensions;
using IntegrationProject.Models;

namespace IntegrationProject.Mappers
{
    public class ProductsMapper : ClassMap<Product>
    {
        public ProductsMapper()
        {
            Map(m => m.Id).Name("ID");
            Map(m => m.Sku).Name("SKU");
            Map(m => m.Name).Name("name");
            Map(m => m.Ean).Name("EAN");
            Map(m => m.ProducerName).Name("producer_name");
            Map(m => m.Category).Name("category");

            // Covert is wire to bool
            Map(m => m.IsWire).Convert(args =>
            {
                var value = args.Row.GetField("is_wire");

                return value?.Trim() == "1";
            });

            Map(m => m.Shipping).Name("shipping");
            
            // Covert avaiable to bool
            Map(m => m.Available).Convert(args =>
            {
                var value = args.Row.GetField("available");

                return value?.Trim() == "1";
            });

            // Covert is vendor to bool
            Map(m => m.IsVendor).Convert(args =>
            {
                var value = args.Row.GetField("is_vendor");

                return value?.Trim() == "1";
            });

            Map(m => m.DefaultImage).Name("default_image");
        }
    }
}
