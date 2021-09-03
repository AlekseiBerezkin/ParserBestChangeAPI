using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public class RatesValue
    {
        public double r1 { get; set; }
        public double r2 { get; set; }
        public double r3 { get; set; }
        public double r4 { get; set; }
        public double r5 { get; set; }

        public void setRate(List<double> l)
        {
            try
            {
                r1 = l.Max(point => point);
                l.Remove(r1);
                r2 = l.Max(point => point);
                l.Remove(r2);
                r3 = l.Max(point => point);
                l.Remove(r3);
                r4 = l.Max(point => point);
                l.Remove(r4);
                r5 = l.Max(point => point);
                l.Remove(r5);
            }
            catch
            {

            }


        }
    }
}
