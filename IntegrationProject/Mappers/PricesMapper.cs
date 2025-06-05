using CsvHelper.Configuration;
using IntegrationProject.Extensions;
using IntegrationProject.Models;

namespace IntegrationProject.Mappers
{
    public class PricesMapper : ClassMap<Prices>
    {
        public PricesMapper()
        {
            Map(m => m.Sku).Index(1);

            Map(m => m.NettPrice).Convert(args =>
            {
                return args.Row.GetField(2).ParseStringToDecimal();
            });

            Map(m => m.NettPriceDiscount).Convert(args =>
            {
                return args.Row.GetField(3).ParseStringToDecimal();
            });

            Map(m => m.NettPriceLogisticDiscount).Convert(args =>
            {
                return args.Row.GetField(5).ParseStringToDecimal();
            });
        }
    }
}
