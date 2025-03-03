
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
            double open,
            double high,
            double low,
            double close,
            long volume,
            double? adjClose = null)
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
        public double Open { get; set; }

        [Required]
        [JsonPropertyName("High")]
        public double High { get; set; }

        [Required]
        [JsonPropertyName("Low")]
        public double Low { get; set; }

        [Required]
        [JsonPropertyName("Close")]
        public double Close { get; set; }


        // Adjusted close will always have a value when getting, but may be null when setting
        private double? _adjClose;

        [JsonPropertyName("AdjClose")]
        public double AdjClose
        {
            get { _adjClose ??= Close; return _adjClose.Value; }
            set { _adjClose = value; }
        }

        [Required]
        [JsonPropertyName("Volume")]
        public long Volume { get; set; }

        public override string ToString()
        {
            string ToStr(double v) => v.ToString("0.000");

            return $"{Timestamp.StdToString()} " +
                $"O:{ToStr(Open)} H:{ToStr(High)} L:{ToStr(Low)} C:{ToStr(Close)} V:{Volume} A:{AdjClose}";
        }
    }
}
