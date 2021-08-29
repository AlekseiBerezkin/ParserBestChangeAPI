using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ParserBestChangeAPI.Model;

namespace ParserBestChangeAPI.Controllers
{
    [ApiController]
    
    public class WeatherForecastController : ControllerBase
    {
        [Route("data")]
        [HttpGet]
        public async Task<object> Get()
        {

            State.stopTimer = 0;
            if(!State.result.Any() )
            {
                TimerControl.TimerStart();
                return "start";
            }
            else if(State.stopTimer == 3)
            {
                
                TimerControl.TimerContinue();
                return "start";
            }
            else
            {
                return State.result;
            }
        }

        [Route("status")]
        [HttpGet]
        public async Task<object> GetStatus()
        {
            return State.flagProcessUpdate;
        }
    }
}
