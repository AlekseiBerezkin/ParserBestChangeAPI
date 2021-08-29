using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public static class State
    {
        public static List<Rates> result = new List<Rates>();

        public static int stopTimer = 0;

        public static bool flagProcessUpdate = false;
    }
}
