
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bot.Models
{
    public class Tick
    {
        public Tick()
        { }

        public Tick(string symbol)
        {
            Symbol = symbol;
        }

        public Tick(
            string symbol,
            TickInterval interval,
            DateTime dateTime,
            double open,
            double high,
            double low,
            double close,
            int volume)
        {
            Symbol = symbol;
            DateTime = dateTime.NormalizeToTickInterval(interval);
            TickInterval = interval;
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
        public TickInterval TickInterval { get; set; }

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
            return $"{Symbol} {DateTime.StandardToString()} " +
                $"Open:{Open} High:{High} Low:{Low} " +
                $"Close:{Close} Volume:{Volume}";
        }
    }
}
