
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theo.Models
{
    public class Bar
    {
        public Bar()
        { }

        public Bar(string symbol)
        {
            Symbol = symbol;
        }

        public Bar(
            string symbol,
            DataInterval interval,
            DateTime dateTime,
            double open,
            double high,
            double low,
            double close,
            int volume)
        {
            Symbol = symbol;
            DateTime = dateTime.NormalizeToBarInterval(interval);
            BarInterval = interval;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        [Key]
        [Column(Order = 1)]
        [Required]
        public string Symbol { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        public DateTime DateTime { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required]
        public DataInterval BarInterval { get; set; }

        [Required]
        public double Open { get; set; }

        [Required]
        public double High { get; set; }

        [Required]
        public double Low { get; set; }

        [Required]
        public double Close { get; set; }

        [Required]
        public int Volume { get; set; }

        public override string ToString()
        {
            string ToStr(double v) => v.ToString("0.00");

            return $"{Symbol} {DateTime.StandardToString()} " +
                $"Open:{ToStr(Open)} High:{ToStr(High)} Low:{ToStr(Low)} " +
                $"Close:{ToStr(Close)} Volume:{ToStr(Volume)}";
        }
    }
}
