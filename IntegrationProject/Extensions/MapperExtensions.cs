using System.Globalization;
using Microsoft.Extensions.Hosting;

namespace IntegrationProject.Extensions
{
    internal static class MapperExtensions
    {
        /// <summary>
        /// Parse input into decimal, unssuccesfull parse return default decimal value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static decimal ParseStringToDecimal(this string? input)
        {
            if (input == null) 
            {
                return default;
            }

            input = input.Trim().Replace(',', '.');

            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                ? result
                : default;
        }

        internal static bool IsStringAsHoursLessOrEqualExcepted(this string? input, int expectedHours)
        {
            // When we do not have data we cant say that is product shipped in time
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            input            = input.Trim().ToLower().Replace("h", "");
            var isParseToInt = int.TryParse(input, out int valueInt);

            if (!isParseToInt)
            {
                return false;
            }

            return valueInt <= expectedHours;
        }
    }
}
