using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public class RatesDouble
    {
        public string Name { get; set; }

        public double currencyLeft { get; set; }
        public double currencyCenter { get; set; }
        public double currencyRights { get; set; }

        internal object CompareTo(RatesDouble b)
        {
            throw new NotImplementedException();
        }
    }
}
