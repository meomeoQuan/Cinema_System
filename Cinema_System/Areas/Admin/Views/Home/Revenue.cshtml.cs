using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cinema_System.Areas.Admin.Views.Home
{
    public class RevenueModel : PageModel
    {
        public decimal[] MonthlyRevenue { get; set; }

        public async Task OnGetAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://localhost:7115/api/home/getmonthlyrevenue");
                MonthlyRevenue = JsonConvert.DeserializeObject<decimal[]>(response);
            }
        }
    }
}