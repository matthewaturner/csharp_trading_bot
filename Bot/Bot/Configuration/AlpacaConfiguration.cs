
namespace Bot.Configuration
{
    public class AlpacaConfiguration
    {
        /// <summary>
        /// The id of the api key.
        /// </summary>
        public string PaperApiKeyId { get; set; }

        /// <summary>
        /// Secret for calling the alpaca api.
        /// </summary>
        public string PaperApiKey { get; set; }

        /// <summary>
        /// Base url for calling Alpaca paper account.
        /// </summary>
        public string PaperApiBaseUrl { get; set; }

        /// <summary>
        /// Base url for calling Alpaca live.
        /// </summary>
        public string ApiBaseUrl { get; set; }
    }
}
