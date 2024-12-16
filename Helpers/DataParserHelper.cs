using DotnetAPI.Models;

namespace DotnetAPI.Helpers
{
    public static class DataParserHelper
    {
        /// <summary>
        /// Parses a raw monthly breakdown string into a list of MonthData objects.
        /// </summary>
        /// <param name="rawData">The raw string containing month and count pairs.</param>
        /// <returns>A list of MonthData objects.</returns>
        public static List<MonthData> ParseMonthlyData(string? rawData)
        {
            var monthlyData = new List<MonthData>();
            if (!string.IsNullOrWhiteSpace(rawData))
            {
                monthlyData = rawData
                    .Split(',')
                    .Select(item =>
                    {
                        var parts = item.Split(':'); // Split "MonthName:Value" pairs
                        return new MonthData
                        {
                            Month = parts[0].Trim(),
                            Count = decimal.TryParse(parts[1].Trim(), out decimal value)
                                ? value
                                : 0,
                        };
                    })
                    .ToList();
            }

            return monthlyData;
        }
    }
}
