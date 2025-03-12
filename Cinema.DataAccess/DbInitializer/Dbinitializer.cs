
using Cinema.DataAccess.Data;
using Cinema.DbInitializer;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cinema.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }


        public void Initialize()
        {


            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }


            if(!_roleManager.RoleExistsAsync(SD.Role_Guest).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Staff)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Guest)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FullName = "Admin",
                    PhoneNumber = "1112223333",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                }, "Admin@123").GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "staff@gmail.com",
                    Email = "staff@gmail.com",
                    FullName = "Staff",
                    PhoneNumber = "1112223333",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                }, "Staff@123").GetAwaiter().GetResult();

                //"123*" is too weak (many ASP.NET Identity systems require at least one 
                //    uppercase letter, one lowercase letter, a number, and a special character). -> can not create user

                // Refresh the user context
                _db.SaveChanges();  // Ensure database changes are committed
                var user = _userManager.FindByEmailAsync("admin@gmail.com").GetAwaiter().GetResult();
                var user2 = _userManager.FindByEmailAsync("staff@gmail.com").GetAwaiter().GetResult();

                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
                if (user2 != null)
                {
                    _userManager.AddToRoleAsync(user2, SD.Role_Staff).GetAwaiter().GetResult();
                }
            }



        }
    }
}