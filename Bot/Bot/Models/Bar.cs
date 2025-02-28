
using Bot.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Bot.Models
{
    public class Bar
    {
        public Bar()
        { }

        public Bar(
            DateTime dateTime,
            string symbol,
            decimal open,
            decimal high,
            decimal low,
            decimal close,
            long volume,
            decimal? adjClose = null)
        {
            Timestamp = dateTime;
            Symbol = symbol;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            _adjClose = adjClose;
        }

        [Required]
        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }

        [Required]
        [JsonPropertyName("Symbol")]
        public string Symbol { get; set; }

        [Required]
        [JsonPropertyName("Open")]
        public decimal Open { get; set; }

        [Required]
        [JsonPropertyName("High")]
        public decimal High { get; set; }

        [Required]
        [JsonPropertyName("Low")]
        public decimal Low { get; set; }

        [Required]
        [JsonPropertyName("Close")]
        public decimal Close { get; set; }


        // Adjusted close will always have a value when getting, but may be null when setting
        private decimal? _adjClose;

        [JsonPropertyName("AdjClose")]
        public decimal AdjClose
        {
            get { _adjClose ??= Close; return _adjClose.Value; }
            set { _adjClose = value; }
        }

        [Required]
        [JsonPropertyName("Volume")]
        public long Volume { get; set; }

        public override string ToString()
        {
            string ToStr(decimal v) => v.ToString("0.000");

            return $"{Timestamp.StdToString()}  " +
                $"O:{ToStr(Open)} H:{ToStr(High)} L:{ToStr(Low)} C:{ToStr(Close)} V:{Volume} A:{AdjClose}";
        }
    }
}
