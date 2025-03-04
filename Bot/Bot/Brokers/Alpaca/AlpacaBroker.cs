using Bot.Brokers.Alpaca.Models;
using Bot.Engine;
using Bot.Exceptions;
using Bot.Models;
using Bot.Models.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bot.Brokers.Alpaca;

public class AlpacaBroker : IBroker
{
    private readonly string apiKeyId;
    private readonly string apiKeySecret;

    private ITradingEngine engine;
    private RestClient restClient;
    private string baseUrl;

    /// <summary>
    /// Constructor which allows for dependency injection on startup.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="keyVaultManager"></param>
    /// <param name="httpClient"></param>
    public AlpacaBroker()
    {
        apiKeyId = GlobalConfig.GetValue("Alpaca:ApiKey");
        apiKeySecret = GlobalConfig.GetValue("Alpaca:ApiKeySecret");

        /*
        baseUrl = config.RunMode switch
        {
            RunMode.BackTest => throw new NotImplementedException(),
            RunMode.Live => config.ApiBaseUrl,
            RunMode.Paper => config.PaperApiBaseUrl,
            _ => throw new NotImplementedException(),
        };
        */

        restClient = new RestClient(baseUrl);

        throw new NotImplementedException("Fix the broker.");
    }

    /// <summary>
    /// Initialize method called by the engine when a backtest begins.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="args"></param>
    public void Initialize(ITradingEngine engine)
    {
        this.engine = engine;
    }

    /// <summary>
    /// Sends an authenticated http request to the alpaca api.
    /// </summary>
    /// <param name="request"></param>
    private RestResponse SendAuthenticatedHttpRequest(RestRequest request, bool validate = true)
    {
        request.AddHeader("APCA-API-KEY-ID", apiKeyId);
        request.AddHeader("APCA-API-SECRET-KEY", apiKeySecret);

        RestResponse response = restClient.Execute(request);

        if (!response.IsSuccessful && validate)
        {
            throw new RestException($"Failure calling alpaca resource {request.Resource}. " +
                $"Status code {response.StatusCode}. " +
                $"Body '{response.Content}'");
        }

        return response;
    }

    /// <summary>
    /// Gets information for an asset like whether it is easy to borrow.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public IAssetInformation GetAssetInformation(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        RestRequest request = new RestRequest($"/v2/assets/{symbol}", Method.Get);
        RestResponse response = SendAuthenticatedHttpRequest(request);
        return JsonConvert.DeserializeObject<AlpacaAssetInformation>(response.Content);
    }

    /// <summary>
    /// Gets account information from alpaca.
    /// </summary>
    /// <returns></returns>
    public IAccount GetAccount()
    {
        RestRequest request = new RestRequest("/v2/account", Method.Get);
        RestResponse response = SendAuthenticatedHttpRequest(request);
        return JsonConvert.DeserializeObject<AlpacaAccount>(response.Content);
    }

    /// <summary>
    /// Gets all currently held positions.
    /// </summary>
    /// <returns></returns>
    public IList<IPosition> GetPositions()
    {
        RestRequest request = new RestRequest("/v2/positions", Method.Get);
        RestResponse response = SendAuthenticatedHttpRequest(request);
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

        RestRequest request = new RestRequest($"/v2/positions/{symbol}", Method.Get);
        RestResponse response = SendAuthenticatedHttpRequest(request);
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

        RestRequest request = new RestRequest($"/v2/orders/{orderId}", Method.Get);
        RestResponse response = SendAuthenticatedHttpRequest(request, validate: false);

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
        RestRequest request = new RestRequest($"/v2/orders", Method.Get);

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

        RestResponse response = SendAuthenticatedHttpRequest(request);
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
    public IOrder PlaceOrder(IOrderRequest orderRequest)
    {
        AlpacaOrderRequest alpacaRequest = new AlpacaOrderRequest(orderRequest);

        //string request_string = JsonConvert.SerializeObject(alpacaRequest);

        RestRequest request = new RestRequest($"/v2/orders", Method.Post);
        request.AddJsonBody(alpacaRequest);
        RestResponse response = SendAuthenticatedHttpRequest(request);
        return JsonConvert.DeserializeObject<AlpacaOrder>(response.Content);
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

        RestRequest request = new RestRequest($"/v2/orders/{orderId}", Method.Delete);
        RestResponse response = SendAuthenticatedHttpRequest(request);
        JsonConvert.DeserializeObject<AlpacaOrder>(response.Content);
        return;
    }

    /// <summary>
    /// Close a position.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public IOrder ClosePosition(string symbol)
    {
        RestRequest request = new RestRequest($"/v2/positions/{symbol}", Method.Delete);
        RestResponse response = SendAuthenticatedHttpRequest(request);
        return JsonConvert.DeserializeObject<AlpacaOrder>(response.Content);
    }
}
