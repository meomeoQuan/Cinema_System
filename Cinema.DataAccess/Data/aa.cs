using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Data
{
    public static class aa
    {
        public static async Task SeedUsers(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    FullName = "Admin User",
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
