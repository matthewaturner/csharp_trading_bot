using Bot.Indicators;
using Bot.Engine;
using System;
using System.Collections.Generic;
using Bot.Models;

namespace Bot.Strategies
{
    public class SMACrossoverStrategy : StrategyBase, IStrategy
    {
        private ITicks ticks;
        private IBroker broker;
        private IList<IIndicator> indicators;
        private IIndicator mac;
        private Position currentPosition;
        private bool longOnly;
        private int maxUnits = 10;
        private string orderId;
        private string symbol;

        public SMACrossoverStrategy(ITicks ticks)
        {
            this.ticks = ticks ?? throw new ArgumentNullException(nameof(ticks));
        }

        public void Initialize(IBroker broker, string[] args)
        {
            symbol = args[0];
            int shortMa = int.Parse(args[1]);
            int longMa = int.Parse(args[2]);
            longOnly = bool.Parse(args[3]);

            this.broker = broker;
            mac = new MovingAverageCrossover(
                shortMa,
                longMa,
                (Tick t) => t.AdjClose);
            indicators = new List<IIndicator> { mac };
        }

        public override IList<IIndicator> Indicators => indicators;

        /// <summary>
        /// Enter Long position if the Moving average crossover value is greater than 0,
        /// Enter Short position if the Moving average crossover value is less than 0,
        /// Exit any position if the Moving average crossover value is 0
        /// </summary>
        /// <param name="tick"></param>
        public void OnTick()
        {
            Tick tick = ticks[symbol];
            mac.OnTick(tick);

            if (Hydrated)
            {
                foreach (var order in broker.OpenOrders)
                {
                    if (order.OrderId == orderId)
                    {
                        broker.CancelOrder(orderId);
                        orderId = null;
                    }
                }

                currentPosition = broker.Portfolio.Positions.ContainsKey(tick.Symbol) ? broker.Portfolio.Positions[tick.Symbol] : null;
                int macVal = (int)mac.Value;
                

                if (macVal == 0 && currentPosition != null)
                {
                    //Exit any position
                    ExitPosition(tick);
                }
                else if (macVal > 0 && (currentPosition == null || currentPosition.GetPositionType() != PositionType.Long))
                {
                    //Enter long position
                    EnterLongPosition(tick);
                }
                else if (macVal < 0 && (currentPosition == null || currentPosition.GetPositionType() != PositionType.Short))
                {
                    if (!longOnly)
                    {
                        //Enter short position
                        EnterShortPosition(tick);
                    }
                    else if (currentPosition != null)
                    {
                        ExitPosition(tick);
                    }
                }
            }            
        }

        public void ExitPosition(Tick tick)
        {
            var orderType = currentPosition.GetPositionType() == PositionType.Long ? OrderType.Sell : OrderType.Buy;
            var quantity = currentPosition.Quantity;
            var targetPrice = tick.AdjClose;
            var order = new OrderRequest(
                orderType,
                currentPosition.Symbol,
                quantity,
                targetPrice
                );
            broker.PlaceOrder(order);
        }

        public void EnterLongPosition(Tick tick)
        {
            double quantity;
            if (currentPosition != null)
            {
                if (longOnly)
                {
                    quantity = maxUnits;
                }
                else
                {
                    quantity = 2 * currentPosition.Quantity * -1;
                }
            }
            else
            {
                quantity = maxUnits;
            }

            var targetPrice = tick.AdjClose;
            var order = new OrderRequest(
                OrderType.Buy,
                tick.Symbol,
                quantity,
                targetPrice
                );
            orderId = broker.PlaceOrder(order);
        }

        public void EnterShortPosition(Tick tick)
        {
            var quantity = currentPosition != null ? 2 * currentPosition.Quantity * -1 : maxUnits;
            var targetPrice = tick.AdjClose;
            var order = new OrderRequest(
                OrderType.Sell,
                tick.Symbol,
                quantity,
                targetPrice
                );
            orderId = broker.PlaceOrder(order);
        }        
    }
}
