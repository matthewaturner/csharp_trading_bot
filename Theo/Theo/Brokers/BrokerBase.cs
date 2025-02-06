using Theo.Engine.Events;
using Theo.Engine;

namespace Theo.Brokers
{
    public abstract class BrokerBase : IInitialize
    {
        public void Initialize(ITradingEngine engine)
        {
            Engine = engine;
        }

        protected ITradingEngine Engine { get; set; }
    }
}