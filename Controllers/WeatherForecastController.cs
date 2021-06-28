using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TcpChecker.Models;

namespace TcpChecker.Controllers
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
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        //[Route("~/book")]
        //[Route("~/book/list")]
        //[Route("ListAll")]
        [Route("[action]")]
        public IActionResult Books()
        {
            CommonLog.Info("Get all books...");

            string lastAccessTime = "";

            var cache = CacheHelper.OpenCache("test");
            if (cache != null)
            {
                lastAccessTime = cache.StringGet("test_key");
                cache.StringSet("test_key", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), TimeSpan.FromSeconds(60));
                if (String.IsNullOrEmpty(lastAccessTime)) lastAccessTime = "This is your fist time to get data";
                else lastAccessTime = "Your last access time is " + lastAccessTime;
            }

            using (var db = DbHelper.OpenDb("test"))
            {
                List<Book> books = db.Fetch<Book>("select * from tbl_book");
                return Ok(new
                {
                    msg = lastAccessTime,
                    items = books
                });
            }
        }
    }
}
