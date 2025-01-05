namespace DotnetAPI.Models
{
    public class Budget
    {
        public string MonthName { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal ActiveBudget { get; set; }
        public decimal ExitedBudget { get; set; }

        public Budget()
        {
            if (MonthName == null)
            {
                MonthName = "";
            }
        }
    }
}
