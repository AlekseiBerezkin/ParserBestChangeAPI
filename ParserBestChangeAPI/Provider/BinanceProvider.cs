using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using Newtonsoft.Json;
using ParserBestChangeAPI.Model;
using static CryptoExchange.Net.ExchangeInterfaces.IExchangeClient;

namespace ParserBestChangeAPI.Provider
{
    public class BinanceProvider
    {
        List<BPair> Bp = new List<BPair>();
        List<BPair> BpUsdt = new List<BPair>();
        BPair busdrub;
        public BinanceProvider()
        {
            RequestDataBinance();
        }

        public Dictionary<string, string> dicBaseCur()
        {
            Dictionary<string, string> dicBase=new Dictionary<string, string>();
            foreach (BPair bp in BpUsdt) 
            {
                if( bp.symbol=="EURUSDT" || bp.symbol == "USDTUAH")
                {
                    dicBase.Add(bp.symbol.Replace("USDT",""),bp.askPrice);
                }
            }
            dicBase.Add(busdrub.symbol.Replace("BUSD", ""), busdrub.askPrice);
            return dicBase;
        }

        public List<BPair> GetBaseCur()
        {
            foreach(BPair bp in Bp)
            {
                if(bp.symbol.Contains("USDT"))
                {
                    BpUsdt.Add(bp);
                }
                if(bp.symbol== "BUSDRUB")
                {
                    busdrub=bp;
                }
            }
            return BpUsdt;
        }

        private void RequestDataBinance()
        {
            

            string request = $"https://api.binance.com/api/v3/ticker/bookTicker";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(request);
            try
            {
                Program.Logger.Info("Запрос на Binance");
                string response;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                Bp = JsonConvert.DeserializeObject<List<BPair>>(response);
                Program.Logger.Info("Ответ получен и обработан");
            }
            catch(Exception ex)
            {
                Program.Logger.Info("Исключение"+ex);
            }
        }

        
    }
}
