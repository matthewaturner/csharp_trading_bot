using Bot.Configuration;
using Bot.Data;
using Bot.Engine;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BotIntegrationTests
{
    [TestClass]
    public class YahooDataSourceTests
    {
        public YahooDataSource yahoo;
        public ITradingEngine engine;

        [TestInitialize]
        public void Setup()
        {
            YahooDataConfig yahooConfig = new YahooDataConfig
            {
                BaseUrl = "https://query1.finance.yahoo.com/",
                OutputDirectory = "data"
            };

            HttpClient httpClient = new HttpClient();

            Mock<IOptionsSnapshot<YahooDataConfig>> yahooConfigSnapshot = new Mock<IOptionsSnapshot<YahooDataConfig>>();
            yahooConfigSnapshot.Setup(m => m.Value).Returns(yahooConfig);

            yahoo = new YahooDataSource(yahooConfigSnapshot.Object, httpClient);
        }
    }
}
