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
            Program.Logger.Info("Возобновление работы");
            timer.Change(0, 60000);
        }
        private static async void updateData(object obj)
        {

            try
            {
                BinanceProvider bp = new BinanceProvider();

                List<BPair> BinanceData= bp.GetBaseCur();
                Dictionary<string, string> basecur= bp.dicBaseCur();
                
                Program.Logger.Info("Обновление даннных");
                State.flagProcessUpdate = true;

                Loader l = new Loader();
                l.DownloadInfoZip();
                // return null;
                Program.Logger.Info("Загрузка файла");
                ZipArchiveProvider zap = new ZipArchiveProvider("info.zip");
                await zap.GetMassData("bm_rates.dat");

                Dictionary<string, List<double>> plus = zap.dictionaryPlus;
                Dictionary<string, List<double>> minus = zap.dictionaryMinus;
                Program.Logger.Info("Преобразования");
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
                Program.Logger.Info("Замена id на имена");
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

                foreach(BPair bpair in BinanceData)
                {
                    string partBinance = bpair.symbol.Replace("USDT", "");

                    for(int i=0;i<ratesMinus.Count;i++)
                    {
                        string[] splitdata = ratesMinus[i].Name.Split(":");
                        if (splitdata[1].Contains("RUB") || splitdata[1].Contains("EUR") || splitdata[1].Contains("UAH"))
                        {
                            if (splitdata[0].Contains(partBinance))
                            {
                                ratesMinus[i].askPrice = bpair.askPrice;
                            }
                        }
                        else if (splitdata[1].Contains(partBinance))
                        {
                            ratesMinus[i].askPrice = bpair.askPrice;
                        }
                        
                    }   
                }


                foreach (BPair bpair in BinanceData)
                {
                    string partBinance = bpair.symbol.Replace("USDT", "");
                   
                    for (int i = 0; i < ratesPlus.Count; i++)
                    {
                        string[] splitdata = ratesPlus[i].Name.Split(":");
                        if (splitdata[0].Contains("RUB") || splitdata[1].Contains("EUR") || splitdata[1].Contains("UAH"))
                        {
                            if (splitdata[1].Contains(partBinance))
                            {
                                ratesPlus[i].askPrice = bpair.askPrice;
                            }
                        }
                        else if (splitdata[0].Contains(partBinance))
                        {
                            ratesPlus[i].askPrice = bpair.askPrice;
                        }

                    }
                }
                /*foreach (BPair bpair in BinanceData)
                {
                    for (int i = 0; i < ratesPlus.Count; i++)
                    {
                        string[] splitdata = ratesPlus[i].Name.Split(":");
                        if (splitdata[0].Contains(bpair.symbol.Replace("USDT", "")))
                        {
                            ratesPlus[i].askPrice = bpair.askPrice;
                            
                        }
                    }
                }*/

                Program.Logger.Info("Получение времени");
                string datetime = await zap.GetDataTimeUpdate("bm_info.dat");
                Program.Logger.Info("Запись данных в файл");

                string jsonbasecur = JsonConvert.SerializeObject(basecur.ToArray());
                File.WriteAllText(@"basecur.json", jsonbasecur);

                string jsonMinus = JsonConvert.SerializeObject(ratesMinus.ToArray());
                File.WriteAllText(@"pathMinus.json", jsonMinus);

                string jsonPlus = JsonConvert.SerializeObject(ratesPlus.ToArray());
                File.WriteAllText(@"pathPlus.json", jsonPlus);

                string DT = JsonConvert.SerializeObject(datetime);
                File.WriteAllText(@"pathDT.json", DT);
                Program.Logger.Info("Запись данных в файл окончена");
                State.flagProcessUpdate = false;
                State.stopTimer++;

                if (State.stopTimer == Settings.timeStop)
                {
                    Program.Logger.Info("Остановка сервиса");
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
                Program.Logger.Info("Исключение при преобразовании"+ex);
                State.flagProcessUpdate = false;
                // return ex.ToString();

            }
        }
    }
}
