using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Configuration
{
    public class YahooDataConfig
    {
        /// <summary>
        /// Base url to query yahoo.
        /// </summary>
        public string BaseUrl { get; set; }
            = "https://query1.finance.yahoo.com/";
    }
}
