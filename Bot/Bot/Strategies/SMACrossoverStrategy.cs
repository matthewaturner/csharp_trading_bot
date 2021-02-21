using Bot.Indicators;
using Bot.Engine;
using System;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public class SMACrossoverStrategy : StrategyBase, IStrategy
    {
        private ITicks ticks;
        private IList<IIndicator> indicators;
        private IIndicator mac;
        private int longLookback;
        private bool longOnly;
        private IBroker broker;
        private Position currentPosition;
        private int maxUnits = 10;
        private string orderId;
        private string symbol;
        private ITicks ticks;

        public SMACrossoverStrategy(ITicks ticks)
        {
            this.ticks = ticks ?? throw new ArgumentNullException(nameof(ticks));
        }

        public void Initialize(string symbol, int shortLookback, int longLookback, bool longOnly, IBroker broker)
        {
            this.longLookback = longLookback;
            this.mac = new MovingAverageCrossover(
                shortLookback,
                longLookback,
                (Tick t) => t.AdjClose);
            this.indicators = new List<IIndicator> { mac };
            this.longOnly = true;
            this.broker = broker;
            this.symbol = symbol;
            this.longOnly = longOnly;
        }

        public override IList<IIndicator> Indicators => indicators;

        /// <summary>
        /// Enter Long position if the Moving average crossover value is greater than 0,
        /// Enter Short position if the Moving average crossover value is less than 0,
        /// Exit any position if the Moving average crossover value is 0
        /// </summary>
        /// <param name="tick"></param>
        public override void OnTick()
        {
            Tick tick = this.ticks[this.symbol];
            this.mac.OnTick(tick);

            if (this.Hydrated)
            {
                foreach (var order in this.broker.OpenOrders)
                {
                    if (order.OrderId == this.orderId)
                    {
                        this.broker.CancelOrder(this.orderId);
                        this.orderId = null;
                    }
                }

                this.currentPosition = this.broker.Portfolio.Positions.ContainsKey(tick.Symbol) ? this.broker.Portfolio.Positions[tick.Symbol] : null;
                int macVal = (int)mac.Value;
                

                if (macVal == 0 && this.currentPosition != null)
                {
                    //Exit any position
                    this.ExitPosition(tick);
                }
                else if (macVal > 0 && (this.currentPosition == null || this.currentPosition.GetPositionType() != PositionType.Long))
                {
                    //Enter long position
                    this.EnterLongPosition(tick);
                }
                else if (macVal < 0 && (this.currentPosition == null || this.currentPosition.GetPositionType() != PositionType.Short))
                {
                    if (!this.longOnly)
                    {
                        //Enter short position
                        this.EnterShortPosition(tick);
                    }
                    else if (this.currentPosition != null)
                    {
                        this.ExitPosition(tick);
                    }
                }
            }            
        }

        public void ExitPosition(Tick tick)
        {
            var orderType = this.currentPosition.GetPositionType() == PositionType.Long ? OrderType.Sell : OrderType.Buy;
            var quantity = this.currentPosition.Quantity;
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
            if (this.currentPosition != null)
            {
                if (this.longOnly)
                {
                    quantity = maxUnits;
                }
                else
                {
                    quantity = 2 * this.currentPosition.Quantity * -1;
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
            this.orderId = broker.PlaceOrder(order);
        }

        public void EnterShortPosition(Tick tick)
        {
            var quantity = this.currentPosition != null ? 2 * this.currentPosition.Quantity * -1 : this.maxUnits;
            var targetPrice = tick.AdjClose;
            var order = new OrderRequest(
                OrderType.Sell,
                tick.Symbol,
                quantity,
                targetPrice
                );
            this.orderId = broker.PlaceOrder(order);
        }
    }
}
