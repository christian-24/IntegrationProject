namespace IntegrationProject.Models
{
    public class PricesModel
    {
        public string Sku                         { get; set; }
        public decimal? NettPrice                 { get; set; }
        public decimal? NettPriceDiscount         { get; set; }
        public decimal? NettPriceLogisticDiscount { get; set; }
    }
}
