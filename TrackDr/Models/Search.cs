using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackDr.Models
{
    public class Search
    {
        public string UserInput { get; set; }
        public string UserState { get; set; }
        public List<string> UserBaseInsurance { get; set; }
    }
}
