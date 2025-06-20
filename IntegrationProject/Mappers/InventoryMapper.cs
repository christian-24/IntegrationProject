﻿using CsvHelper.Configuration;
using IntegrationProject.Extensions;
using IntegrationProject.Models;

namespace IntegrationProject.Mappers
{
    public class InventoryMapper : ClassMap<InventoryModel>
    {
        public InventoryMapper()
        {
            Map(m => m.ProductId).Name("product_id");
            Map(m => m.Unit).Name("unit");

            Map(m => m.Qty).Convert(args =>
            {
                return args.Row.GetField("qty").ParseStringToDecimal();
            });

            Map(m => m.Manufacturer).Name("manufacturer_name");

            Map(m => m.IsShippingIn24Hours).Convert(args =>
            {
                return args.Row.GetField("shipping").IsStringAsHoursLessOrEqualExcepted(24);
            });

            Map(m => m.ShippingCost).Convert(args =>
            {
                return args.Row.GetField("shipping_cost").ParseStringToDecimal();
            });
        }
    }
}
