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

            Map(m => m.IsWire).Convert(args =>
            {
                return args.Row.GetField("is_wire").ParseStringAsBool();
            });

            Map(m => m.Shipping).Name("shipping");
            
            Map(m => m.Available).Convert(args =>
            {
                return args.Row.GetField("available").ParseStringAsBool();
            });

            Map(m => m.IsVendor).Convert(args =>
            {
                return args.Row.GetField("is_vendor").ParseStringAsBool();
            });

            Map(m => m.DefaultImage).Name("default_image");
        }
    }
}
