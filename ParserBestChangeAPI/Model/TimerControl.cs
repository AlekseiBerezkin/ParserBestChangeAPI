using ParserBestChangeAPI.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public static class TimerControl
    {

        // устанавливаем метод обратного вызова
        static TimerCallback tm = new TimerCallback(updateData);
        // создаем таймер
        static Timer timer;

        static public void TimerStart()
        {
            timer = new Timer(tm, 0, 0, 30000);
        }

        static public void TimerContinue()
        {
            timer.Change(0, 30000);
        }
        private static async void updateData(object obj)
        {
            try
            {
                State.flagProcessUpdate = true;
                State.stopTimer++;

                if(State.stopTimer==3)
                {
                    timer.Change(System.Threading.Timeout.Infinite, 0);
                }

                Loader.DownloadInfoZip();
                   // return null;

                ZipArchiveProvider zap = new ZipArchiveProvider("info.zip");
                Dictionary<string, List<double>> z = await zap.GetMassData("bm_rates.dat");

                var keysDictionary = z.Keys;


                foreach (string pair in keysDictionary.ToList())
                {
                    if (!(z[pair].Count <= 5))
                    {
                        z[pair].Sort((a, b) => a.CompareTo(b));
                        z[pair] = z[pair].GetRange(z[pair].Count - 5, 5);
                    }
                }



                Dictionary<string, List<double>> listCalc = new Dictionary<string, List<double>>();

                foreach (string pair in keysDictionary.ToList())
                {
                    List<double> calc = new List<double>();


                    for (int i = z[pair].Count - 1; i > 0; i--)
                    {

                        calc.Add(((z[pair][z[pair].Count - 1] - z[pair][i - 1]) / z[pair][z[pair].Count - 1]) * 100);
                    }
                    listCalc.Add(pair, calc);
                }

                List<Rates> listRates = new List<Rates>();
                foreach (string pair in keysDictionary.ToList())
                {
                    if (!(listCalc[pair].Count == 0))
                    {
                        listRates.Add(new Rates { Name = pair, Rate = listCalc[pair] });
                    }

                }
                z.Clear();
                listRates.Sort((a, b) => a.Rate[a.Rate.Count - 1].CompareTo(b.Rate[b.Rate.Count - 1]));
                listRates.AddRange(listRates.GetRange(listRates.Count - 1000, 1000));

                List<Rates> listRatesPlus = new List<Rates>();
                List<Rates> listRatesMinus = new List<Rates>();
                listRatesPlus.AddRange(listRates.GetRange(listRates.Count - Settings.Sample, Settings.Sample));
                listRatesMinus.AddRange(listRates.GetRange(0, Settings.Sample));
                listRates.Clear();
                listRates.AddRange(listRatesMinus);
                listRates.AddRange(listRatesPlus);

                Dictionary<string, string> currency = await zap.GetCurrencys("bm_cy.dat");

                IdexToName itn = new IdexToName(currency);

                for (int i = 0; i < listRates.Count; i++)
                {
                    listRates[i].Name = itn.convertIdToName(listRates[i].Name);
                }

                CreateLink cl = new CreateLink("currency-link.txt");

                for (int i = 0; i < listRates.Count; i++)
                {
                    listRates[i].url = cl.getLink(listRates[i].Name);
                }

                State.result=listRates;
                State.flagProcessUpdate = false;
            }
            catch (Exception ex)
            {
                State.flagProcessUpdate = false;
                // return ex.ToString();

            }
        }

    }
}
