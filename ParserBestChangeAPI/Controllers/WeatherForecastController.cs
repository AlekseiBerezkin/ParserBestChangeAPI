using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ParserBestChangeAPI.Provider;
using System.Web;
using System.Text;
using ParserBestChangeAPI.Model;

namespace ParserBestChangeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {

                if (!Loader.DownloadInfoZip())
                    return null;

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
                //z.Clear();
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
                //listRatesPlus.Reverse();
                listRates.AddRange(listRatesPlus);

                Dictionary<string, string> currency = await zap.GetCurrencys("bm_cy.dat");

                IdexToName itn = new IdexToName(currency);

                for(int i=0;i<listRates.Count;i++)
                {
                    listRates[i].Name= itn.convertIdToName(listRates[i].Name);
                }

                CreateLink cl = new CreateLink("currency-link.txt");

                for (int i = 0; i < listRates.Count; i++)
                {
                    listRates[i].url = cl.getLink(listRates[i].Name);
                }
                //listCalc.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                //List<Rates> zbs = new List<Rates>();
                //var result = z.GroupBy(x => x).Select(x => new { Name = x.Key, Count = x.Count() });
                /*zbs = z.FindAll(
delegate (Rates r)
{
    return r.ID1=="1";
}
);*/
                int[] array = new int[11];
                array[10] = 1;
                return listRates;




                //return file;
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }

        }
    }
}
