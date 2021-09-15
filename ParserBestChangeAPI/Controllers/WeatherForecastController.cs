using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ParserBestChangeAPI.Model;
using System.IO;
using Newtonsoft.Json;

namespace ParserBestChangeAPI.Controllers
{
    [ApiController]
    
    public class WeatherForecastController : ControllerBase
    {
        [Route("dataMinus")]
        [HttpGet]
        public async Task<object> GetMinus()
        {
            State.stopTimer = 0;
                using (StreamReader reader = new StreamReader("pathMinus.json"))
                {
                    var r = reader.ReadToEnd();
                    return r;
                }
        }

        [Route("basecur")]
        [HttpGet]
        public async Task<object> GetBaseCur()
        {
            
            using (StreamReader reader = new StreamReader("basecur.json"))
            {
                var r = reader.ReadToEnd();
                return r;
            }

        }

        [Route("dataPlus")]
        [HttpGet]
        public async Task<object> GetPlus()
        {
            
            State.stopTimer = 0;
                using (StreamReader reader = new StreamReader("pathPlus.json"))
                {
                    var r = reader.ReadToEnd();
                    return r;
                }
        }

        [Route("datetime")]
        [HttpGet]
        public async Task<object> GetDT()
        {
            using (StreamReader reader = new StreamReader("pathDT.json"))
            {
                var r = reader.ReadToEnd();
                return r;
            }
        }


    }
}
