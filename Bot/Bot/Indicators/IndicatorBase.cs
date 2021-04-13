
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Indicators
{
    public abstract class IndicatorBase : IIndicator
    {
        public IDictionary<string, double> Values;

        /// <summary>
        /// Constructor.
        /// </summary>
        public IndicatorBase()
        {
            Values = new Dictionary<string, double>();
        }

        /// <summary>
        /// How long before indicator is hydrated.
        /// </summary>
        public virtual int Lookback { get; set; }

        /// <summary>
        /// Whether the indicator is hydrated.
        /// </summary>
        public virtual bool Hydrated { get; set; }

        /// <summary>
        /// Default value of indicator. Useful when only one value is needed,
        /// which is most of the time.
        /// </summary>
        public virtual double Value
        {
            get
            {
                return this["default"];
            }
        }

        /// <summary>
        /// Accessor for named values the indicator may output.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[string valueName]
        {
            get
            {
                if (!Hydrated)
                {
                    return double.NaN;
                }

                return Values[valueName];
            }
        }

        /// <summary>
        /// Calculate new values.
        /// </summary>
        /// <param name="ticks"></param>
        public abstract void OnTick(IMultiTick ticks);
    }
}
