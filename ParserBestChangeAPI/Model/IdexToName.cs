using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public class IdexToName
    {
         private Dictionary<string, string> currency { get; set; }
        public IdexToName(Dictionary<string, string> c)
        {
            currency = c;
        }
        public string convertIdToName(string pair)
        {
            string[] splitData = pair.Split("-");
            return currency[splitData[0]] + ":" + currency[splitData[1]];
        }
    }
}
