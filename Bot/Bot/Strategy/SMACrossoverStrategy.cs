using Bot.Indicators;
using Bot.Indicators.Interfaces;
using Bot.Models;
using Bot.Engine;
using Bot.Brokers;
using Bot.Models.Interfaces;
using System.Linq;

namespace Bot.Strategies
{
    public class SMACrossoverStrategy : StrategyBase, IStrategy
    {
        private IMovingAverageCrossover mac;
        private IPosition currentPosition;
        private bool longOnly;
        private int maxUnits = 10;
        private string orderId;
        private string symbol;

        public SMACrossoverStrategy(
            string symbol,
            int shortLookback,
            int longLookback,
            bool longOnly)
            : base()
        { 
            mac = new MovingAverageCrossover(
                shortLookback,
                longLookback,
                (IMultiTick t) => t[symbol].Close);
            Indicators.Add(mac);
        }

        private IBroker broker => Engine.Broker;

        /// <summary>
        /// Enter Long position if the Moving average crossover value is greater than 0,
        /// Enter Short position if the Moving average crossover value is less than 0,
        /// Exit any position if the Moving average crossover value is 0
        /// </summary>
        /// <param name="_"></param>
        public override void OnTick(IMultiTick ticks)
        {
            Tick tick = ticks[symbol];
            System.Console.WriteLine(tick.Close);

            if (IsHydrated)
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
            var targetPrice = tick.Close;
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

            var targetPrice = tick.Close;
            var order = new OrderRequest(
                OrderType.MarketBuy,
                tick.Symbol,
                quantity,
                targetPrice);
            orderId = broker.PlaceOrder(order).OrderId;
        }

        private void EnterShortPosition(Tick tick)
        {
            var quantity = currentPosition != null ? 2 * currentPosition.Quantity * -1 : maxUnits;
            var targetPrice = tick.Close;
            var order = new OrderRequest(
                OrderType.MarketSell,
                tick.Symbol,
                quantity,
                targetPrice);
            orderId = broker.PlaceOrder(order).OrderId;
        }        
    }
}
