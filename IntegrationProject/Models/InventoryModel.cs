
namespace IntegrationProject.Models
{
    public class InventoryModel
    {
        public int ProductId            { get; set; } // Unique ID of the product
        public string Unit              { get; set; } // Type of logistic unit the product is sold as
        public decimal Qty              { get; set; }
        public string? Manufacturer     { get; set; }
        public bool IsShippingIn24Hours { get; set; } // Is shipping time in 24 hours
        public decimal ShippingCost     { get; set; } // Net cost for shipping the product
    }
}
