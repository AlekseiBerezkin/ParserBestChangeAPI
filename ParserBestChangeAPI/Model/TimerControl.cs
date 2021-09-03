using ParserBestChangeAPI.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;

namespace ParserBestChangeAPI.Model
{
    public class TimerControl
    {

        // устанавливаем метод обратного вызова
        static TimerCallback tm = new TimerCallback(updateData);
        // создаем таймер
        static Timer timer;

        static public void TimerStart()
        {
            timer = new Timer(tm, 0, 0, 60000);
        }

        static public void TimerContinue()
        {
            timer.Change(0, 60000);
        }
        private static async void updateData(object obj)
        {

            try
            {
                
                State.flagProcessUpdate = true;

                Loader l = new Loader();
                l.DownloadInfoZip();
                   // return null;

                ZipArchiveProvider zap = new ZipArchiveProvider("info.zip");
                await zap.GetMassData("bm_rates.dat");

                Dictionary<string, List<double>> plus = zap.dictionaryPlus;
                Dictionary<string, List<double>> minus = zap.dictionaryMinus;

                foreach (string pair in minus.Keys.ToList())
                {
                    if (minus[pair].Count >= 5)
                    {
                        minus[pair].Sort((a, b) => a.CompareTo(b));
                        minus[pair] = minus[pair].GetRange(0, 5);
                    }
                }

               
                List<Rates> ratesMinus = new List<Rates>();

                foreach (var zz in minus)
                {

                    Rates r = new Rates()
                    {
                        Name=zz.Key,
                        Rate=zz.Value
                    };

                    if(!(r.Rate.Count < 2))
                    {
                        ratesMinus.Add(r);
                    }
                    
                }


                foreach (string pair in plus.Keys.ToList())
                {
                    if (plus[pair].Count >= 5)
                    {
                        plus[pair].Sort((a, b) => a.CompareTo(b));
                        plus[pair] = plus[pair].GetRange(plus[pair].Count - 5, 5);
                    }
                }


                List<Rates> ratesPlus = new List<Rates>();

                foreach (var zz in plus)
                {

                    Rates r = new Rates()
                    {
                        Name = zz.Key,
                        Rate = zz.Value
                    };

                    if (!(r.Rate.Count < 2))
                    {
                        ratesPlus.Add(r);
                    }

                }

                Dictionary<string, string> currency = await zap.GetCurrencys("bm_cy.dat");

                IdexToName itn = new IdexToName(currency);

                for (int i = 0; i < ratesMinus.Count; i++)
                {
                    ratesMinus[i].Name = itn.convertIdToName(ratesMinus[i].Name);
                }
                for (int i = 0; i < ratesPlus.Count; i++)
                {
                    ratesPlus[i].Name = itn.convertIdToName(ratesPlus[i].Name);
                }

                CreateLink cl = new CreateLink("currency-link.txt");

                for (int i = 0; i < ratesMinus.Count; i++)
                {
                    ratesMinus[i].url = cl.getLink(ratesMinus[i].Name);
                }

                for (int i = 0; i < ratesPlus.Count; i++)
                {
                    ratesPlus[i].url = cl.getLink(ratesPlus[i].Name);
                }

                string datetime = await zap.GetDataTimeUpdate("bm_info.dat");

                string jsonMinus = JsonConvert.SerializeObject(ratesMinus.ToArray());
                File.WriteAllText(@"pathMinus.json", jsonMinus);

                string jsonPlus = JsonConvert.SerializeObject(ratesPlus.ToArray());
                File.WriteAllText(@"pathPlus.json", jsonPlus);

                string DT = JsonConvert.SerializeObject(datetime);
                File.WriteAllText(@"pathDT.json", DT);

                State.flagProcessUpdate = false;
                State.stopTimer++;

                if (State.stopTimer == Settings.timeStop)
                {
                    timer.Change(Timeout.Infinite, 0);
                    State.flagStateServer = false;
                }
                else
                {
                    State.flagStateServer = true;
                }
                
            }
            catch (Exception ex)
            {
                State.flagProcessUpdate = false;
                // return ex.ToString();

            }
        }
    }
}
