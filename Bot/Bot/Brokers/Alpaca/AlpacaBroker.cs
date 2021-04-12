
using Bot.Brokers.Alpaca.Models;
using Bot.Configuration;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Interfaces;
using Core.Azure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly AlpacaConfiguration config;
        private ITradingEngine engine;
        private string baseUrl;

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
            this.config = config.Value;
            apiKeyId = keyVaultManager.GetSecretAsync(config.Value.PaperApiKeyIdSecretName).Result;
            apiKeySecret = keyVaultManager.GetSecretAsync(config.Value.PaperApiKeySecretName).Result;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Initialize method called by the engine when a backtest begins.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        /// <param name="args[0]">Bool indicating paper trading api (true) or live api (false).</param>
        public void Initialize(ITradingEngine engine, string[] args)
        {
            this.engine = engine;
            baseUrl = bool.Parse(args[0]) ? config.PaperApiBaseUrl : config.ApiBaseUrl;
        }

        /// <summary>
        /// Sends an authenticated http request to the alpaca api.
        /// </summary>
        /// <param name="request"></param>
        private HttpResponseMessage SendAuthenticatedHttpRequest(HttpRequestMessage request)
        {
            request.Headers.Add("APCA-API-KEY-ID", apiKeyId);
            request.Headers.Add("APCA-API-SECRET-KEY", apiKeySecret);

            HttpResponseMessage response = httpClient.Send(request);
            response.EnsureSuccessStatusCode();

            return response;
        }

        /// <summary>
        /// Gets account information from alpaca.
        /// </summary>
        /// <returns></returns>
        public IAccount GetAccount()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl + "/v2/account");
                HttpResponseMessage response = SendAuthenticatedHttpRequest(request);

                return JsonConvert.DeserializeObject<AlpacaAccount>(response.Content.ReadAsStringAsync().Result);
            }
            catch (HttpRequestException ex)
            {
                engine.Log($"Caught exception in method {nameof(GetAccount)} : {ex}");
            }
            catch (JsonSerializationException ex)
            {
                engine.Log($"Failed to deserialize AlpacaAccount : {ex}");
            }

            return null;
        }

        public IList<IPosition> GetPositions()
        {
            throw new NotImplementedException();
        }

        public IPosition GetPosition(string symbol)
        {
            throw new NotImplementedException();
        }

        public double GetCashBalance()
        {
            throw new NotImplementedException();
        }

        public IOrder GetOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public IList<IOrder> GetOrdersByState(OrderState state)
        {
            throw new NotImplementedException();
        }

        public IList<IOrder> GetOpenOrders()
        {
            return GetOrdersByState(OrderState.Open);
        }

        public IList<IOrder> GetAllOrders()
        {
            throw new NotImplementedException();
        }

        public double GetPortfolioValue()
        {
            throw new NotImplementedException();
        }

        public void OnTick(IMultiTick ticks)
        {
            throw new NotImplementedException();
        }

        public string PlaceOrder(IOrderRequest order)
        {
            throw new NotImplementedException();
        }

        public void CancelOrder(string orderId)
        {
            throw new NotImplementedException();
        }
    }
}
