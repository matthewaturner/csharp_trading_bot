
using Bot.Configuration;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Interfaces;
using Core.Azure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Bot.Brokers
{
    public class AlpacaBroker : IBroker
    {
        private readonly string apiKeyId;
        private readonly string apiKeySecret;
        private readonly HttpClient httpClient;
        private ITradingEngine engine;

        /// <summary>
        /// Constructor which allows for dependency injection on startup.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="keyVaultManager"></param>
        /// <param name="httpClient"></param>
        public AlpacaBroker(
            IOptionsSnapshot<AlpacaConfiguration> config,
            IKeyVaultManager keyVaultManager,
            HttpClient httpClient)
        {
            apiKeyId = keyVaultManager.GetSecretAsync(config.Value.ApiKeyIdSecretName).Result;
            apiKeySecret = keyVaultManager.GetSecretAsync(config.Value.ApiKeySecretName).Result;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Initialize method called by the engine when a backtest begins.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, string[] args)
        {
            this.engine = engine;
        }

        public IList<Order> OpenOrders => throw new NotImplementedException();

        public IList<Order> OrderHistory => throw new NotImplementedException();

        public void CancelOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public IAccount GetAccount()
        {
            throw new NotImplementedException();
        }

        public IList<IPosition> GetPositions()
        {
            throw new NotImplementedException();
        }

        public IPosition GetPosition(string symbol)
        {
            throw new NotImplementedException();
        }

        public double CashBalance()
        {
            throw new NotImplementedException();
        }

        public Order GetOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public IList<Order> GetOrderHistory(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public void OnTick(IMultiTick ticks)
        {
            throw new NotImplementedException();
        }

        public string PlaceOrder(OrderRequest order)
        {
            throw new NotImplementedException();
        }

        public double PortfolioValue()
        {
            throw new NotImplementedException();
        }
    }
}
