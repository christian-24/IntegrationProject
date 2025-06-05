using IntegrationProject.Extensions;

namespace IntegrationProject.Tests.Extensions
{
    public class MapperExtensionsTests
    {
        [Theory]
        [InlineData("123,45", 123.45)]
        [InlineData("78.9", 78.9)]
        [InlineData("  5,5  ", 5.5)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("invalid", 0)]
        public void ParseStringToDecimal_ShouldConvertCorrectly(string? input, decimal expected)
        {
            var result = input.ParseStringToDecimal();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("24h", 24, true)]
        [InlineData("72h", 24, false)]
        [InlineData("1", 2, true)]
        [InlineData("Na zamówienie", 24, false)]
        [InlineData("", 24, false)]
        [InlineData(null, 24, false)]
        [InlineData("48H", 48, true)]
        public void IsStringAsHoursLessOrEqualExcepted_ShouldReturnExpected(string? input, int expectedMax, bool expected)
        {
            var result = input.IsStringAsHoursLessOrEqualExcepted(expectedMax);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData(" 1 ", true)]
        [InlineData("0", false)]
        [InlineData("true", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ParseStringAsBool_ShouldInterpretCorrectly(string? input, bool expected)
        {
            var result = input.ParseStringAsBool();

            Assert.Equal(expected, result);
        }
    }
}
