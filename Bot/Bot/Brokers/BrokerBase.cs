using Bot.Engine.Events;
using Bot.Engine;

namespace Bot.Brokers
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