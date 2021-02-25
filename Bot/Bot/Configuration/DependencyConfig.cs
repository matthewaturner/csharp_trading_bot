using Newtonsoft.Json;

namespace Bot.Configuration
{
    public class DependencyConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("args")]
        public string[] Args { get; set; }
    }
}
