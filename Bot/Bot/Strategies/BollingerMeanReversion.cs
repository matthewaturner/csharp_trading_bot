using Bot.Engine;
using Bot.Indicators;
using Bot.Models;
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
        private BollingerBand bollingerBand;
        private IList<IIndicator> indicators;
        private string[] symbols;
        private double[] hedgeRatios;
        private Position position;

        public BollingerMeanReversion()
        {
            indicators = new List<IIndicator>();
        }

        public override IList<IIndicator> Indicators => indicators;

        public void Initialize(ITradingEngine engine, string[] args)
        {
            this.engine = engine;

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
            indicators.Add(bollingerBand);
            position = Position.Neutral;
        }

        public double HedgedPortfolioValue(ITicks ticks)
        {
            double value = 0;
            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double hedgeRatio = hedgeRatios[i];

                value += ticks[symbol].AdjClose * hedgeRatio;
            }
            return value;
        }

        public double HedgedTradeValue(ITicks ticks)
        {
            double value = 0;
            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double hedgeRatio = hedgeRatios[i];

                value += Math.Abs(ticks[symbol].AdjClose * hedgeRatio);
            }
            return value;
        }

        public void EnterPositions(int longOrShort)
        {
            double tradeableValue = engine.Broker.PortfolioValue() * .95;
            double unitPortfolioValue = HedgedTradeValue(engine.Ticks);
            double numUnitPortfolios = tradeableValue / unitPortfolioValue;

            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double currentPrice = engine.Ticks[symbol].AdjClose;
                double desiredUnits = numUnitPortfolios * hedgeRatios[i] * longOrShort;

                double currentUnits = 0;
                if (engine.Broker.Portfolio.Positions.ContainsKey(symbol))
                {
                    currentUnits = engine.Broker.Portfolio.Positions[symbol].Quantity;
                }

                double difference = desiredUnits - currentUnits;
                if (difference > 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.Buy,
                        symbol,
                        Math.Abs(difference),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
                else if (difference < 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.Sell,
                        symbol,
                        Math.Abs(difference),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
            }
        }

        public void ExitPositions()
        {
            for (int i=0; i<symbols.Length; i++)
            {
                string symbol = symbols[i];
                double currentPrice = engine.Ticks[symbol].AdjClose;

                double currentUnits = 0;
                if (engine.Broker.Portfolio.Positions.ContainsKey(symbol))
                {
                    currentUnits = engine.Broker.Portfolio.Positions[symbol].Quantity;
                }

                if (currentUnits > 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.Sell,
                        symbol,
                        Math.Abs(currentUnits),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
                else if (currentUnits < 0)
                {
                    OrderRequest order = new OrderRequest(
                        OrderType.Buy,
                        symbol,
                        Math.Abs(currentUnits),
                        currentPrice);
                    engine.Broker.PlaceOrder(order);
                }
            }
        }

        public void OnTick(ITicks ticks)
        {
            bollingerBand.OnTick(ticks);

            if (Hydrated)
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
}
