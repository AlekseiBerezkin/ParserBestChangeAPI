using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public class ParserPair: IDisposable
    {
        private Dictionary<string, List<double>> source;
        public ParserPair(Dictionary<string, List<double>> z)
        {
            source = z;
        }

        public Dictionary<string, List<double>> GetTopFiveDictionary()
        {
            Dictionary<string, List<double>> dicCur = source;
            foreach (string pair in GetKeysDictionary().ToList())
            {
                if (!(dicCur[pair].Count <= 5))
                {
                    dicCur[pair].Sort((a, b) => a.CompareTo(b));
                    dicCur[pair] = dicCur[pair].GetRange(dicCur[pair].Count - 5, 5);
                }
            }

            return dicCur;
        }

        public Dictionary<string, List<double>>.KeyCollection  GetKeysDictionary()
        {
            return source.Keys;
        }

        public void Dispose()
        {
            
        }
    }
}
