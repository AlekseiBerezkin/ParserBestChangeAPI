using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public class Rates
    {
        public string Name { get; set;}
        public List<double> Rate { get; set; }
        public string url { get; set; } = "";
        public double back { get; set; }
        public string askPrice { get; set; } = "0";
    }
}
