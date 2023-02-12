
namespace Bot.Configuration
{
    public class AlpacaConfig
    {
        /// <summary>
        /// The id of the api key.
        /// </summary>
        public string ApiKeyId { get; set; }

        /// <summary>
        /// Secret for calling the alpaca api.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Base url for calling Alpaca paper account.
        /// </summary>
        public string PaperApiBaseUrl { get; set; } 
            = "https://paper-api.alpaca.markets/";

        /// <summary>
        /// Base url for calling Alpaca live.
        /// </summary>
        public string ApiBaseUrl { get; set; } 
            = "https://api.alpaca.markets/";

        /// <summary>
        /// Run mode for the alpaca api.
        /// </summary>
        public RunMode RunMode { get; set; }
    }
}
