using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public class ParseTxt
    {
        private List<string> Currency = new List<string>();
        public ParseTxt(string FileName)
        {
            using (StreamReader reader = new StreamReader(FileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Currency.Add(line);
                }
            }
        }

        public bool ConteinCurrency(string cur)
        {
            return Currency.Contains(cur);
        }
    }
}
