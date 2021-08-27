using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Provider
{
    public static class Loader
    {

        public static bool DownloadInfoZip()
        {
            try
            {
                WebClient webClient = new WebClient();
                {
                    webClient.DownloadFile("http://api.bestchange.ru/info.zip","info.zip");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
