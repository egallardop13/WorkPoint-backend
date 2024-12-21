namespace DotnetAPI.Models
{
    public class MetricsInfo
    {
        public int TotalEmployees { get; set; }
        public int JoinedOrLeftYearly { get; set; }
        public IEnumerable<MonthData>? MonthlyBreakdown { get; set; }
    }

    public class MonthData
    {
        public string? Month { get; set; }
        public decimal Count { get; set; }
    }
}
