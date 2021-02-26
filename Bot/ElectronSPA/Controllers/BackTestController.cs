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
        public OrderHistory RunBackTest()
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
            tradingEngine.RunAsync(engineConfig);
            /*
            var dates = tradingEngine.Broker.OrderHistory.Select(x => x.ExecutionTime).OrderBy(x => x.Date);

            var minDate = dates.Min();
            var maxDate = dates.Max();
            var dateArray = Enumerable.Range(0, 1 + maxDate.Subtract(minDate).Days)
                .Select(offset => minDate.AddDays(offset)).OrderBy(x => x.Date).ToArray();
            */
            var orders = tradingEngine.Broker.OrderHistory.OrderBy(x => x.ExecutionTime).ToArray();
            var dateArray = orders.Select(x => x.ExecutionTime).ToArray();
            var portfolioValueArray = orders.Select(x => x.ExecutionPrice * x.Quantity).ToArray();
            var quantityArray = orders.Select(x => x.Quantity).ToArray();
            /*
            for (int i = 0; i < dateArray.Length; i++)
            {
                if (orders[i].ExecutionTime == dateArray[i])
                {
                    portfolioValueArray[i] = orders[i].Quantity * orders[i].ExecutionPrice;
                    quantityArray[i] = orders[i].Quantity;
                }
                else
                {
                    portfolioValueArray[i] = portfolioValueArray[i - 1];
                    quantityArray[i] = 0;
                }                
            }
            */
            var orderHistory = new OrderHistory()
            {
                symbol = orders[0].Symbol,
                dates = dateArray,
                portfolioValue = portfolioValueArray,
                quantity = quantityArray
            };

            return orderHistory;
        }
    }
}
