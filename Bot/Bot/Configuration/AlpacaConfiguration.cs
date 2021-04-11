
namespace Bot.Configuration
{
    public class AlpacaConfiguration
    {
        /// <summary>
        /// The id of the api key.
        /// </summary>
        public string ApiKeyIdSecretName { get; set; }

        /// <summary>
        /// Secret for calling the alpaca api.
        /// </summary>
        public string ApiKeySecretName { get; set; }
    }
}
