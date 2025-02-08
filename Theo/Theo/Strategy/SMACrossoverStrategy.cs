using Theo.Indicators;
using Theo.Indicators.Interfaces;
using Theo.Models;
using Theo.Engine;
using Theo.Brokers;
using Theo.Models.Interfaces;
using System.Linq;

namespace Theo.Strategies
{
    public class SMACrossoverStrategy : StrategyBase, IStrategy
    {
        private IMovingAverageCrossover mac;
        private IPosition currentPosition;
        private bool longOnly;
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
                (MultiBar t) => t[symbol].Close);
            Indicators.Add(mac);

            this.symbol = symbol;
            this.longOnly = longOnly;
        }

        private IBroker broker => Engine.Broker;

        /// <summary>
        /// Enter Long position if the Moving average crossover value is greater than 0,
        /// Enter Short position if the Moving average crossover value is less than 0,
        /// Exit any position if the Moving average crossover value is 0
        /// </summary>
        /// <param name="_"></param>
        public override void OnBar(MultiBar bars)
        {
            Bar bar = bars[symbol];
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

                currentPosition = broker.GetPositions().FirstOrDefault(pos => pos.Symbol.Equals(bar.Symbol));
                int macVal = (int)mac.Value;

                if (macVal == 0 && currentPosition != null)
                {
                    Engine.Logger.LogInformation("Exiting positions!");
                    ExitPosition(bar);
                }
                else if (macVal > 0 && (currentPosition == null || currentPosition.Type != PositionType.Long))
                {
                    Engine.Logger.LogInformation("Going long!");
                    EnterLongPosition(bar);
                }
                else if (macVal < 0 && (currentPosition == null || currentPosition.Type != PositionType.Short))
                {
                    if (!longOnly)
                    {
                        Engine.Logger.LogInformation("Going short!");
                        EnterShortPosition(bar);
                    }
                    else if (currentPosition != null)
                    {
                        Engine.Logger.LogInformation("Exiting positions!");
                        ExitPosition(bar);
                    }
                }
            }            
        }

        private void ExitPosition(Bar bar)
        {
            var orderType = currentPosition.Type == PositionType.Long ? OrderType.MarketSell : OrderType.MarketBuy;
            var quantity = currentPosition.Quantity;
            var targetPrice = bar.Close;
            var order = new OrderRequest(
                orderType,
                currentPosition.Symbol,
                quantity,
                targetPrice
                );
            broker.PlaceOrder(order);
        }

        private void EnterLongPosition(Bar bar)
        {
            double quantity = broker.GetAccount().TotalValue / bar.Close - 1;
            var targetPrice = bar.Close;
            var order = new OrderRequest(
                OrderType.MarketBuy,
                bar.Symbol,
                quantity,
                targetPrice);
            orderId = broker.PlaceOrder(order).OrderId;
        }

        private void EnterShortPosition(Bar bar)
        {
            var quantity = broker.GetAccount().TotalValue / bar.Close - 1;
            var targetPrice = bar.Close;
            var order = new OrderRequest(
                OrderType.MarketSell,
                bar.Symbol,
                quantity,
                targetPrice);
            orderId = broker.PlaceOrder(order).OrderId;
        }        
    }
}
