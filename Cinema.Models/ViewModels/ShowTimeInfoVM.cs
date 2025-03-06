using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class ShowTimeInfoVM
    {
        public string ShowDate { get; set; }  // The specific show date
        public List<string> ShowTimes { get; set; } = new List<string>(); // List of available times
    }
}
