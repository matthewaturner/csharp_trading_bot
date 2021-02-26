using Bot.Configuration;
using Bot.Engine;
using Bot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronSPA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackTestController : ControllerBase
    {
        private readonly ILogger<BackTestController> logger;

        private ITradingEngine tradingEngine;

        public BackTestController(ILogger<BackTestController> logger, ITradingEngine tradingEngine)
        {
            this.logger = logger;
            this.tradingEngine = tradingEngine;
        }

        [HttpGet]
        public IEnumerable<int> GetRandomListOfNumbers(int lengthOfArray)
        {
            var arr = new int[lengthOfArray];
            var rng = new Random();

            for (int i = 0; i < lengthOfArray; i++)
            {
                arr[i] = rng.Next(1000);
            }

            return arr;
        }

        [HttpGet]
        public IEnumerable<Order> RunBackTest()
        {
            var engineConfig = new EngineConfig()
            {
                Symbols = new List<string>() { "MSFT" },
                Interval = TickInterval.Day,
                Start = new DateTime(2010, 1, 1),
                End = new DateTime(2021, 1, 1),
                DataSource = new DependencyConfig()
                {
                    Name = "YahooDataSource"
                },
                Broker = new DependencyConfig()
                {
                    Name = "BackTestingBroker",
                    Args = new string[] { "1000" }
                },
                Strategy = new DependencyConfig()
                {
                    Name = "SMACrossOverStrategy",
                    Args = new string[] { "MSFT", "16", "64", "true" }
                },
                Analyzers = new List<DependencyConfig>()
                {
                    new DependencyConfig()
                    {
                        Name = "SharpRatio",
                        Args = new string[] { "0.00005357"}
                    }, 
                }

            };
            tradingEngine.RunAsync(engineConfig);

            return tradingEngine.Broker.OrderHistory;
        }
    }
}
