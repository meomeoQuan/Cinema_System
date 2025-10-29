namespace Cinema.Models.ViewModels
{
    public class RevenueViewModel
    {
        //public List<double> MonthlyRevenue { get; set; }
        public List<double> MonthlyRevenue { get; set; } = new();
        public List<double> LastYearRevenue { get; set; } = new();
        public List<string> MonthLabels { get; set; } = new();
        //public RevenueViewModel()
        //{
        //    MonthlyRevenue = new List<double>(); // Initialize with an empty list
        //}
    }
}
