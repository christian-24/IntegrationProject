using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace IntegrationProject.Helpers
{
    internal class ParserHelper
    {
        public static List<T> ParseCsv<T, TMap>(StreamReader streamReader, string delimeter, bool hasHeader, bool shouldSkipEmptyLine)
            where TMap : ClassMap<T>, new()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter         = delimeter,
                HasHeaderRecord   = hasHeader,
                HeaderValidated   = null,
                MissingFieldFound = null
            };

            if (shouldSkipEmptyLine)
            {
                config.ShouldSkipRecord = args =>
                {
                    var record = args.Row.Parser?.Record;

                    return record == null ||
                           record.All(string.IsNullOrWhiteSpace) ||
                           record.Any(c => c.Contains("__empty_line__"));
                };
            }

            using var csv = new CsvReader(streamReader, config);
            csv.Context.RegisterClassMap<TMap>();

            return csv.GetRecords<T>().ToList();
        }
    }
}
