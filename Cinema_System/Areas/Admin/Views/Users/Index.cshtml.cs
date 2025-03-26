using Cinema.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cinema_System.Areas.Admin.Views.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IndexModel(UserManager<ApplicationUser> userManager) => _userManager = userManager;
        public string CurrentUserId { get; private set; }
        public IEnumerable<ApplicationUser> Users { get; private set; }
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            CurrentUserId = user?.Id;
            Users = _userManager.Users;
        }
    }
}
