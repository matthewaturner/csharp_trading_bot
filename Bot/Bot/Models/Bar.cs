
using Bot.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
            long volume)
        {
            Timestamp = dateTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
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

        [Required]
        [JsonPropertyName("Volume")]
        public long Volume { get; set; }

        public override string ToString()
        {
            string ToStr(double v) => v.ToString("0.000");

            return $"{Timestamp.StdToString()}:: O:{ToStr(Open)} H:{ToStr(High)} L:{ToStr(Low)} C:{ToStr(Close)} V:{Volume}";
        }
    }
}
