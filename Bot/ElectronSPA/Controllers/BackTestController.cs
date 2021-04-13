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
        public async Task<OrderHistory> RunBackTest()
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
                    Name = "SMACrossoverStrategy",
                    Args = new string[] { "MSFT", "16", "64", "true" }
                },
                Analyzers = new List<DependencyConfig>()
                {
                    new DependencyConfig()
                    {
                        Name = "SharpeRatio",
                        Args = new string[] { "0.00005357"}
                    }, 
                }
            };

            tradingEngine.Initialize(engineConfig);
            await tradingEngine.RunAsync();

            Console.WriteLine("Completed running the engine");
            var orders = tradingEngine.Broker.GetAllOrders().OrderBy(x => x.ExecutionTime).ToArray();
            var dateArray = orders.Select(x => x.ExecutionTime).ToArray();
            var portfolioValueArray = orders.Select(x => x.AverageFillPrice * x.Quantity).ToArray();
            var quantityArray = orders.Select(x => x.Quantity).ToArray();

            var orderHistory = new OrderHistory()
            {
                symbol = orders[0].Symbol,
                dates = dateArray,
                portfolioValue = portfolioValueArray,
                quantity = quantityArray
            };

            Console.WriteLine("Returning to front end...");
            return orderHistory;
        }
    }
}
