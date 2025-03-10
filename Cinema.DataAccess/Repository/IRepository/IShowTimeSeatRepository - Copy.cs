using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IShowTimeSeatRepository : IRepository<ShowtimeSeat>
        
    {
       
        void Update(ShowtimeSeat showtimeSeat);
    }
}
