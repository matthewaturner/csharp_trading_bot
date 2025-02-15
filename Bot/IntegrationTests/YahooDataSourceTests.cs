using Bot.Data;
using Bot.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

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
            HttpClient httpClient = new HttpClient();
            yahoo = new YahooDataSource(httpClient);
        }
    }
}
