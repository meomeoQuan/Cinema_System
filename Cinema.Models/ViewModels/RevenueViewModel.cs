namespace Cinema.Models.ViewModels
{
    public class RevenueViewModel
    {
        public List<double> MonthlyRevenue { get; set; } 
        public RevenueViewModel()
        {
            MonthlyRevenue = new List<double>(); // Initialize with an empty list
        }
    }
}
