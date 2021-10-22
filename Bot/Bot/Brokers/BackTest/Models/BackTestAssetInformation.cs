using Bot.Models.Interfaces;

namespace Bot.Brokers.BackTest.Models
{
    class BackTestAssetInformation : IAssetInformation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol"></param>
        public BackTestAssetInformation(string symbol)
        {
            Id = symbol;
            Class = "unknown";
            Exchange = "unknown";
            Symbol = symbol;
            Status = "active";
            Tradable = true;
            Marginable = true;
            Shortable = true;
            EasyToBorrow = true;
            Fractionable = true;
        }

        public string Id { get; set; }

        public string Class { get; set; }

        public string Exchange { get; set; }

        public string Symbol { get; set; }

        public string Status { get; set; }

        public bool Tradable { get; set; }

        public bool Marginable { get; set; }

        public bool Shortable { get; set; }

        public bool EasyToBorrow { get; set; }

        public bool Fractionable { get; set; }
    }
}
