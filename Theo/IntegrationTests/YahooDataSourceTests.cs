using Theo.Data;
using Theo.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace TheoIntegrationTests
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
