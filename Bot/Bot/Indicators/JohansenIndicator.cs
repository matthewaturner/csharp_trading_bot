using Bot.Models;
using RDotNet;
using System;
using System.Collections.Generic;

namespace Bot.Indicators
{
    /// <summary>
    /// Johansen test determines stationarity for a set of price series.
    /// Outputs are whether the series is found to be stationary and the hedge ratios of a stationary portfolio.
    /// </summary>
    public class JohansenIndicator : IndicatorBase
    {
        private REngine engine;
        private IList<double>[] columnData;
        private Func<Tick, double> transform;
        private string[] symbols;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        public JohansenIndicator(
            int lookback,
            string[] symbols,
            Func<Tick, double> transform)
            : base()
        { 
            StartupParameter rinit = new StartupParameter();
            rinit.Quiet = true;
            rinit.RHome = "C:/Program Files/R/R-3.4.3";
            rinit.Interactive = true;
            engine = REngine.GetInstance(null, true, rinit);
            engine.Evaluate("library('urca')");

            Lookback = lookback;
            Hydrated = false;

            this.symbols = symbols;
            this.transform  = transform;
            columnData = new List<double>[symbols.Length];

            for (int i=0; i<symbols.Length; i++)
            {
                columnData[i] = new List<double>();
            }
        }

        /// <summary>
        /// Calculate outputs.
        /// </summary>
        /// <param name="ticks"></param>
        public override void OnTick(IMultiBar ticks)
        {
            // if we aren't hydrated we are just adding to the list then returning
            if (!Hydrated)
            {
                for (int i=0; i<ticks.NumSymbols; i++)
                {
                    columnData[i].Add(transform(ticks[i]));
                }

                if (columnData[0].Count == Lookback)
                {
                    Hydrated = true;
                }
            }
            else
            {
                for (int i=0; i<ticks.NumSymbols; i++)
                {
                    columnData[i].RemoveAt(0);
                    columnData[i].Add(transform(ticks[i]));
                }
            }

            if (Hydrated)
            {
                DataFrame df = engine.CreateDataFrame(columnData);
                engine.SetSymbol("df", df);
                engine.Evaluate("jotest=ca.jo(df, type='trace', K=2, ecdet='none', spec='longrun')");
                engine.Evaluate("evec <- slot(jotest, 'W')[,1]");
                double[] evec = engine.GetSymbol("evec").AsNumeric().ToArray();

                for (int i=0; i<evec.Length; i++)
                {
                    Values[symbols[i]] = evec[i];
                }
                Values["default"] = double.NaN;
            }
        }
    }
}
