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

                //if (!Loader.DownloadInfoZip())
                    //return null;

                ZipArchiveProvider zap = new ZipArchiveProvider("info.zip");
                

                Dictionary<string, string> currency = await zap.GetCurrencys("bm_cy.dat");

                byte[] utf8 = Encoding.UTF8.GetBytes(currency["4"]);

                string g = Encoding.UTF8.GetString(utf8); ;


               
                return currency["4"];
                    



                //return file;
            }
            catch(Exception ex)
            {
                
                return ex.ToString();
            }
            
        }
    }
}
