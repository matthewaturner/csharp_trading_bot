using Bot.DataSources;
using Bot.Engine;
using Bot.Events;

namespace Bot.Brokers;

public abstract class BrokerBase : IInitialize
{
    public void Initialize(ITradingEngine engine)
    {
        Engine = engine;
    }

    protected ITradingEngine Engine { get; set; }

    protected IDataSource DataSource => Engine.DataSource;
}