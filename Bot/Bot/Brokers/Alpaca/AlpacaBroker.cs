
using Bot.Brokers.Alpaca.Models;
using Bot.Configuration;
using Bot.Engine;
using Bot.Exceptions;
using Bot.Models;
using Bot.Models.Interfaces;
using Core.Azure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bot.Brokers
{
    public class AlpacaBroker : IBroker
    {
        private readonly string apiKeyId;
        private readonly string apiKeySecret;
        private readonly AlpacaConfiguration config;

        private ITradingEngine engine;
        private RestClient restClient;
        private string baseUrl;

        /// <summary>
        /// Constructor which allows for dependency injection on startup.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="keyVaultManager"></param>
        /// <param name="httpClient"></param>
        public AlpacaBroker(
            IOptionsSnapshot<AlpacaConfiguration> config,
            IKeyVaultManager keyVaultManager)
        {
            this.config = config.Value;
            apiKeyId = keyVaultManager.GetSecretAsync(config.Value.PaperApiKeyIdSecretName).Result;
            apiKeySecret = keyVaultManager.GetSecretAsync(config.Value.PaperApiKeySecretName).Result;
            restClient = new RestClient();
            restClient.UseNewtonsoftJson();
        }

        /// <summary>
        /// Initialize method called by the engine when a backtest begins.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, RunMode runMode, string[] args)
        {
            this.engine = engine;

            if (runMode == RunMode.BackTest)
            {
                throw new NotImplementedException();
            }

            if (runMode == RunMode.Paper)
            {
                baseUrl = config.PaperApiBaseUrl;
            }

            if (runMode == RunMode.Live)
            {
                baseUrl = config.ApiBaseUrl;
            }

            restClient.BaseUrl = new Uri(baseUrl);
        }

        /// <summary>
        /// Sends an authenticated http request to the alpaca api.
        /// </summary>
        /// <param name="request"></param>
        private IRestResponse SendAuthenticatedHttpRequest(IRestRequest request, bool validate = true)
        {
            request.AddHeader("APCA-API-KEY-ID", apiKeyId);
            request.AddHeader("APCA-API-SECRET-KEY", apiKeySecret);

            IRestResponse response = restClient.Execute(request);

            if (!response.IsSuccessful && validate)
            {
                throw new RestException($"Failure calling alpaca resource {request.Resource}. Status code {response.StatusCode}");
            }

            return response;
        }

        /// <summary>
        /// Gets account information from alpaca.
        /// </summary>
        /// <returns></returns>
        public IAccount GetAccount()
        {
            IRestRequest request = new RestRequest("/v2/account", Method.GET);
            IRestResponse response = SendAuthenticatedHttpRequest(request);
            return JsonConvert.DeserializeObject<AlpacaAccount>(response.Content);
        }

        /// <summary>
        /// Gets all currently held positions.
        /// </summary>
        /// <returns></returns>
        public IList<IPosition> GetPositions()
        {
            IRestRequest request = new RestRequest("/v2/positions", Method.GET);
            IRestResponse response = SendAuthenticatedHttpRequest(request);
            var positions = JsonConvert.DeserializeObject<IList<AlpacaPosition>>(response.Content);
            return positions.ToList<IPosition>();
        }

        /// <summary>
        /// Gets a specific position.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public IPosition GetPosition(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            IRestRequest request = new RestRequest($"/v2/positions/{symbol}", Method.GET);
            IRestResponse response = SendAuthenticatedHttpRequest(request);
            return JsonConvert.DeserializeObject<AlpacaPosition>(response.Content);
        }

        /// <summary>
        /// Gets an order by id.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IOrder GetOrder(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                throw new ArgumentNullException(nameof(orderId));
            }
            
            IRestRequest request = new RestRequest($"/v2/orders/{orderId}", Method.GET);
            IRestResponse response = SendAuthenticatedHttpRequest(request, validate:false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            if (!response.IsSuccessful)
            {
                throw new RestException($"Failure calling alpaca resource {request.Resource}. Status code {response.StatusCode}");
            }

            return JsonConvert.DeserializeObject<AlpacaOrder>(response.Content);
        }

        /// <summary>
        /// Get orders matching some state. (filled, open, etc.)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<IOrder> QueryOrders(
            IEnumerable<string> symbols,
            OrderState state,
            DateTime after,
            DateTime until,
            int limit = 50)
        {
            IRestRequest request = new RestRequest($"/v2/orders", Method.GET);

            if (symbols != null && symbols.Any())
            {
                request.AddQueryParameter("symbols", string.Join(",", symbols));
            }

            if (state != OrderState.Unknown)
            {
                if (state == OrderState.Open)
                {
                    request.AddQueryParameter("status", "open");
                }
                else
                {
                    request.AddQueryParameter("status", "closed");
                }
            }
            request.AddQueryParameter("after", after.ToString("O"));
            request.AddQueryParameter("until", until.ToString("O"));
            request.AddQueryParameter("limit", limit.ToString());

            IRestResponse response = SendAuthenticatedHttpRequest(request);
            return JsonConvert.DeserializeObject<IList<AlpacaOrder>>(response.Content).ToList<IOrder>();
        }

        /// <summary>
        /// Get all open orders.
        /// </summary>
        /// <returns></returns>
        public IList<IOrder> GetOpenOrders()
        {
            return QueryOrders(null, OrderState.Open, DateTime.MinValue, DateTime.MaxValue, 50);
        }

        /// <summary>
        /// Gets all orders.
        /// </summary>
        /// <returns></returns>
        public IList<IOrder> GetAllOrders()
        {
            return QueryOrders(null, OrderState.Unknown, DateTime.MinValue, DateTime.MaxValue, 500);
        }

        /// <summary>
        /// Send an order to alpaca.
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        public string PlaceOrder(IOrderRequest orderRequest)
        {
            AlpacaOrderRequest alpacaRequest = new AlpacaOrderRequest(orderRequest);

            IRestRequest request = new RestRequest($"/v2/orders", Method.POST);
            request.AddJsonBody(alpacaRequest);
            IRestResponse response = SendAuthenticatedHttpRequest(request);
            IOrder newOrder = JsonConvert.DeserializeObject<AlpacaOrder>(response.Content);
            return newOrder.OrderId;
        }

        /// <summary>
        /// Cancel an existing / outstanding order.
        /// </summary>
        /// <param name="orderId"></param>
        public void CancelOrder(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            IRestRequest request = new RestRequest($"/v2/orders/{orderId}", Method.DELETE);
            IRestResponse response = SendAuthenticatedHttpRequest(request);
            IOrder order = JsonConvert.DeserializeObject<AlpacaOrder>(response.Content);
            return;
        }
    }
}
