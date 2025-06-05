using System.Globalization;

namespace IntegrationProject.Extensions
{
    internal static class MapperExtensions
    {
        /// <summary>
        /// Attempts to parse a string to a decimal value.
        /// 
        /// The method:
        /// - Trims the input string,
        /// - Replaces commas with dots to normalize decimal separators,
        /// - Uses invariant culture to ensure consistent parsing,
        /// - Returns 0 if the parsing fails or the input is null.
        /// 
        /// This is useful for parsing price or quantity values from CSV files.
        /// </summary>
        /// <param name="input">The string to parse as a decimal.</param>
        /// <returns>The parsed decimal value, or 0 if parsing failed.</returns>
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

        /// <summary>
        /// Checks whether a given shipping time string (e.g., "24h", "72H") represents a delivery time
        /// that is less than or equal to the expected number of hours.
        /// 
        /// Assumes the input string contains a numeric value optionally followed by "h" or "H".
        /// Trims and lowercases the input before attempting to parse the numeric value.
        /// 
        /// Returns false if the string is null, empty, or cannot be parsed to an integer.
        /// </summary>
        /// <param name="input">Shipping time as a string (e.g., "24h", "72h", "Na zamówienie")</param>
        /// <param name="expectedHours">Maximum allowed shipping time in hours</param>
        /// <returns>True if parsed hours are ≤ expectedHours, false otherwise</returns>
        internal static bool IsStringAsHoursLessOrEqualExcepted(this string? input, int expectedHours)
        {
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

        /// <summary>
        /// Parses a string as a boolean where only the exact string "1" (after trimming) is considered true.
        /// All other values, including null, empty, or any value different from "1", are considered false.
        /// 
        /// This is commonly used to interpret binary flags in CSV data (e.g., "1" = true, "0" or "" = false).
        /// </summary>
        /// <param name="input">Input string to evaluate.</param>
        /// <returns>True if the trimmed input equals "1"; otherwise, false.</returns>
        internal static bool ParseStringAsBool(this string? input)
        {
            return input?.Trim() == "1";
        }
    }
}
