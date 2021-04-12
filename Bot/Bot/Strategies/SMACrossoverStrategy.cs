using Bot.Indicators;
using Bot.Models;
using Bot.Engine;
using Bot.Brokers;
using Bot.Models.Interfaces;
using System.Linq;

namespace Bot.Strategies
{
    public class SMACrossoverStrategy : StrategyBase, IStrategy
    {
        private IBroker broker;
        private IIndicator mac;
        private IPosition currentPosition;
        private bool longOnly;
        private int maxUnits = 10;
        private string orderId;
        private string symbol;

        public SMACrossoverStrategy()
            : base()
        { }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public override void Initialize(ITradingEngine engine, string[] args)
        {
            symbol = args[0];
            int shortMa = int.Parse(args[1]);
            int longMa = int.Parse(args[2]);
            longOnly = bool.Parse(args[3]);

            broker = engine.Broker;
            mac = new MovingAverageCrossover(
                shortMa,
                longMa,
                (IMultiTick t) => t[symbol].AdjClose);
            Indicators.Add(mac);
        }

        /// <summary>
        /// Enter Long position if the Moving average crossover value is greater than 0,
        /// Enter Short position if the Moving average crossover value is less than 0,
        /// Exit any position if the Moving average crossover value is 0
        /// </summary>
        /// <param name="_"></param>
        public override void StrategyOnTick(IMultiTick ticks)
        {
            Tick tick = ticks[symbol];
            mac.OnTick(ticks);

            if (Hydrated)
            {
                foreach (var order in broker.GetOpenOrders())
                {
                    if (order.OrderId == orderId)
                    {
                        broker.CancelOrder(orderId);
                        orderId = null;
                    }
                }

                currentPosition = broker.GetPositions().FirstOrDefault(pos => pos.Symbol.Equals(tick.Symbol));
                int macVal = (int)mac.Value;

                if (macVal == 0 && currentPosition != null)
                {
                    //Exit any position
                    ExitPosition(tick);
                }
                else if (macVal > 0 && (currentPosition == null || currentPosition.Type != PositionType.Long))
                {
                    //Enter long position
                    EnterLongPosition(tick);
                }
                else if (macVal < 0 && (currentPosition == null || currentPosition.Type != PositionType.Short))
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

        private void ExitPosition(Tick tick)
        {
            var orderType = currentPosition.Type == PositionType.Long ? OrderType.MarketSell : OrderType.MarketBuy;
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

        private void EnterLongPosition(Tick tick)
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
                OrderType.MarketBuy,
                tick.Symbol,
                quantity,
                targetPrice
                );
            orderId = broker.PlaceOrder(order);
        }

        private void EnterShortPosition(Tick tick)
        {
            var quantity = currentPosition != null ? 2 * currentPosition.Quantity * -1 : maxUnits;
            var targetPrice = tick.AdjClose;
            var order = new OrderRequest(
                OrderType.MarketSell,
                tick.Symbol,
                quantity,
                targetPrice
                );
            orderId = broker.PlaceOrder(order);
        }        
    }
}
