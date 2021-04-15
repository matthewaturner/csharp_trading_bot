using Bot.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Bot.Configuration
{
    public class EngineConfig
    {
        [JsonProperty("symbols")]
        public IList<string> Symbols { get; set; }

        [JsonProperty("tickInterval")]
        public TickInterval Interval { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("runMode")]
        public RunMode RunMode { get; set; }

        [JsonProperty("dataSource")]
        public DependencyConfig DataSource { get; set; }

        [JsonProperty("broker")]
        public DependencyConfig Broker { get; set; }

        [JsonProperty("strategy")]
        public DependencyConfig Strategy { get; set; }

        [JsonProperty("analyzers")]
        public IList<DependencyConfig> Analyzers { get; set; }
    }
}
