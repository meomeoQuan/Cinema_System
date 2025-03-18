using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(ApplicationUser applicationUser)
        {
           _db.Update(applicationUser);
        }
        public ApplicationUser? FindAdminUserByName(string name)
        {
            var users = _db.ApplicationUsers.ToList();
            return users.FirstOrDefault(u => u.FullName == name);
        }
    } 
}
