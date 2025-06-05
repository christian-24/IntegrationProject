

namespace IntegrationProject.Models
{
    public class Product
    {
        public int Id              { get; set; } // Unique product ID
        public string Sku          { get; set; } // SKU - unique warehouse ID
        public string Name         { get; set; }
        public string Ean          { get; set; }
        public string ProducerName { get; set; }
        public string Category     { get; set; }
        public bool IsWire         { get; set; } // True if product is a wire
        public string Shipping     { get; set; } // Is shipping time in 24 hours
        public bool Available      { get; set; }
        public bool IsVendor       { get; set; } // True if shipped by supplier
        public string DefaultImage { get; set; }
    }
}
