namespace IntegrationProject.Dtos
{
    public class ProductDetailsDto
    {
        public string Name          { get; set; }
        public string Ean           { get; set; }
        public string Category      { get; set; }
        public string DefaultImage  { get; set; }
        public decimal Qty          { get; set; }
        public string Unit          { get; set; }
        public decimal Price        { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
