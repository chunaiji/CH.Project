using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkyApm.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CH.Project.WebApi.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITracingContext tracingContext)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            Commont.LogCommont.SerilogActionExtention.CreateInstance().Info($"时间:{DateTime.Now.ToString()}");
            Commont.LogCommont.SerilogActionExtention.CreateInstance().Debug($"时间:{DateTime.Now.ToString()}");
            Commont.LogCommont.SerilogActionExtention.CreateInstance().Error($"时间:{DateTime.Now.ToString()}");
            Commont.LogCommont.SerilogActionExtention.CreateInstance().Warning($"时间:{DateTime.Now.ToString()}");

            string Ip = this.Request.Headers["X-Real-IP"].FirstOrDefault() ?? this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var newGuid = Guid.NewGuid();
            ImHelper.JoinChan(Guid.NewGuid(), "demoChan");//群聊，绑定消息频道
            ImHelper.PrevConnectServer(newGuid, Ip);

            ImHelper.GetChanList().Select(a => new { a.chan, a.online });
            var temp2 = Guid.NewGuid();
            for (int i = 0; i < 10; i++)
            {
                var temp = Guid.NewGuid();
                temp2 = temp;
                ImHelper.PrevConnectServer(temp, Ip);
                ImHelper.JoinChan(temp, "demoChan");//群聊，绑定消息频道
            }
            ImHelper.SendChanMessage(newGuid, "demoChan", "hello word");
            ImHelper.SendMessage(newGuid, new[] { temp2 }, " SendMessage hello word ", false);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
