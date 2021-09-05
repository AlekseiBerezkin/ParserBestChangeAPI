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
            Program.Logger.Info("Запрос отрицательных данных");
            State.stopTimer = 0;
                using (StreamReader reader = new StreamReader("pathMinus.json"))
                {
                    var r = reader.ReadToEnd();
                    return r;
                }
            
        }

        [Route("dataPlus")]
        [HttpGet]
        public async Task<object> GetPlus()
        {
            Program.Logger.Info("Запрос положительных данных");
            State.stopTimer = 0;
                using (StreamReader reader = new StreamReader("pathPlus.json"))
                {
                    var r = reader.ReadToEnd();
                    return r;
                }
        }

        

        [Route("continue")]
        [HttpGet]
        public void Start()
        {
            Program.Logger.Info("Продолжить преобразования");
            TimerControl.TimerContinue();
        }

        [Route("status")]
        [HttpGet]
        public async Task<object> GetStatus()
        {
            Program.Logger.Info("Запрос статуса");
            return State.flagProcessUpdate;
        }

        [Route("stateServer")]
        [HttpGet]
        public async Task<object> GetState()
        {
            
            return JsonConvert.SerializeObject(State.flagStateServer);
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
