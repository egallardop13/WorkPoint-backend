namespace DotnetAPI.Models
{
    public class Budget
    {
        public string MonthName { get; set; }
        public int TotalBudget { get; set; }
        public int ActiveBudget { get; set; }
        public int ExitedBudget { get; set; }

        public Budget()
        {
            if (MonthName == null)
            {
                MonthName = "";
            }
        }
    }
}
