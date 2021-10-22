using Bot.Brokers;
using Bot.Engine;
using Bot.Indicators;
using Bot.Models;
using Bot.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Strategies
{
    public class BollingerMeanReversion : StrategyBase, IStrategy
    {
        enum Position
        {
            Long = 1,
            Neutral = 0,
            Short = -1
        };

        private ITradingEngine engine;
        private IBroker broker;

        private BollingerBand bollingerBand;
        private string[] symbols;
        private double[] hedgeRatios;
        private Position position;

        /// <summary>
        /// Base constructor.
        /// </summary>
        public BollingerMeanReversion()
            : base()
        { }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public override void Initialize(ITradingEngine engine, string[] args)
        {
            this.engine = engine;
            broker = engine.Broker;

            int lookback = int.Parse(args[0]);
            double entryZScore = double.Parse(args[1]);
            double exitZScore = double.Parse(args[2]);

            symbols = engine.Symbols.ToArray();
            hedgeRatios = args[3].Split(';').Select(str => double.Parse(str)).ToArray();

            if (symbols.Length != hedgeRatios.Length)
            {
                throw new ArgumentException("Hedge ratios array length != symbols array length");
            }

            bollingerBand = new BollingerBand(lookback, entryZScore, exitZScore, HedgedPortfolioValue);
            Indicators.Add(bollingerBand);

            position = Position.Neutral;
        }

        /*
        private Position GetCurrentPosition()
        {
            foreach (IPosition pos in broker.GetPositions())
            {
                string symbol = pos.Symbol;
                int i = Array.FindIndex(symbols, str => str.Equals(symbol, StringComparison.OrdinalIgnoreCase));

                if (i == -1)
                {

                }
            }
        }
        */

        /// <summary>
        /// Fields included in csv output.
        /// </summary>
        /// <returns></returns>
        public override string[] GetCsvHeaders()
        {
            return new string[] { "Unit Portfolio Value", "Position Type" };
        }

        /// <summary>
        /// Fields included in csv output.
        /// </summary>
        /// <returns></returns>
        public override string[] GetCsvValues()
        {
            return new string[] { 
                HedgedPortfolioValue(this.engine.Ticks).ToString(), 
                position.ToString()
            };
        }

        /// <summary>
        /// Calculates the value of the unit portfolio, which is the value of each asset
        /// multiplied by their hedge ratios.
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public double HedgedPortfolioValue(IMultiBar ticks)
        {
            double value = 0;
            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double hedgeRatio = hedgeRatios[i];

                value += ticks[symbol].Close * hedgeRatio;
            }
            return value;
        }

        /// <summary>
        /// Gets the absolute value of the unit portfolio. Used to decide how many portfolios
        /// to buy. Hedged value is often close to 0 and will result in over-leverage.
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public double HedgedTradeValue(IMultiBar ticks)
        {
            double value = 0;
            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double hedgeRatio = hedgeRatios[i];

                value += Math.Abs(ticks[symbol].Close * hedgeRatio);
            }
            return value;
        }

        /// <summary>
        /// Go long or short.
        /// </summary>
        /// <param name="longOrShort"></param>
        public void EnterPositions(int longOrShort)
        {
            double tradeableValue = engine.Broker.GetAccount().Cash * .95;
            double unitPortfolioValue = HedgedTradeValue(engine.Ticks);
            double numUnitPortfolios = tradeableValue / unitPortfolioValue;

            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double currentPrice = engine.Ticks[symbol].Close;
                double desiredUnits = numUnitPortfolios * hedgeRatios[i] * longOrShort;

                double currentUnits = 0;
                IPosition currentPosition = broker.GetPosition(symbol);

                if (currentPosition != null)
                {
                    currentUnits = currentPosition.Quantity;
                }

                double difference = desiredUnits - currentUnits;
                if (difference > 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.MarketBuy,
                        symbol,
                        Math.Abs(difference),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
                else if (difference < 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.MarketSell,
                        symbol,
                        Math.Abs(difference),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
            }
        }

        /// <summary>
        /// Close out all positions.
        /// </summary>
        public void ExitPositions()
        {
            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double currentPrice = engine.Ticks[symbol].Close;

                double currentUnits = 0;
                IPosition currentPosition = broker.GetPosition(symbol);

                if (currentPosition != null)
                {
                    currentUnits = currentPosition.Quantity;
                }

                if (currentUnits > 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.MarketSell,
                        symbol,
                        Math.Abs(currentUnits),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
                else if (currentUnits < 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.MarketBuy,
                        symbol,
                        Math.Abs(currentUnits),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
            }
        }

        /// <summary>
        /// What to do when a new tick of data comes in.
        /// </summary>
        /// <param name="ticks"></param>
        public override void StrategyOnTick(IMultiBar ticks)
        {
            int bollValue = (int)bollingerBand.Value;

            if (bollValue == 0 && position != Position.Neutral)
            {
                ExitPositions();
                position = Position.Neutral;
            }
            else if (bollValue > 0 && position != Position.Long)
            {
                EnterPositions((int)Position.Long);
                position = Position.Long;
            }
            else if (bollValue < 0 && position != Position.Short)
            {
                EnterPositions((int)Position.Short);
                position = Position.Short;
            }
        }
    }
}
