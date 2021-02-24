
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bot.Brokers
{
    public class Tick
    {
        public Tick()
        { }

        public Tick(
            string symbol,
            TickInterval interval,
            DateTime dateTime,
            double open,
            double high,
            double low,
            double close,
            double adjClose,
            int volume)
        {
            Symbol = symbol;
            DateTime = dateTime.NormalizeToTickInterval(interval);
            TickInterval = interval;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            AdjClose = adjClose;
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

        public double AdjOpen { get { return AdjValue(Open); } }

        [Required]
        public double High { get; set; }

        public double AdjHigh { get { return AdjValue(High); } }

        [Required]
        public double Low { get; set; }

        public double AdjLow { get { return AdjValue(Low); } }

        [Required]
        public double Close { get; set; }

        [Required]
        public double AdjClose { get; set; }

        [Required]
        public int Volume { get; set; }

        public override string ToString()
        {
            return $"{Symbol} {DateTime.StandardToString()} " +
                $"Open:{Open} High:{High} Low:{Low} " +
                $"Close:{Close} AdjClose:{AdjClose} Volume:{Volume}";
        }

        public string PrimaryKey()
        {
            return Symbol + DateTime.ToString("O") + TickInterval.ToString();
        }

        private double AdjValue(double value)
        {
            if (AdjClose == 0.0)
            {
                return value;
            }
            return (AdjClose / Close) * value;
        }
    }
}
