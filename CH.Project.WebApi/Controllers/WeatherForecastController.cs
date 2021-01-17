using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            Commont.LogCommont.SerilogActionExtention.CreateInstantiation().Info($"时间:{DateTime.Now.ToString()}");
            Commont.LogCommont.SerilogActionExtention.CreateInstantiation().Debug($"时间:{DateTime.Now.ToString()}");
            Commont.LogCommont.SerilogActionExtention.CreateInstantiation().Error($"时间:{DateTime.Now.ToString()}");
            Commont.LogCommont.SerilogActionExtention.CreateInstantiation().Warning($"时间:{DateTime.Now.ToString()}");
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
