
using Bot.DataSources;
using Bot.Engine;
using Bot.Events;
using Microsoft.Extensions.Logging;
using System;

namespace Bot.Brokers;

public abstract class BrokerBase : IInitializeReceiver
{
    protected ILogger logger;

    protected ITradingEngine Engine { get; set; }

    protected IDataSource DataSource => Engine.DataSource;

    /// <summary>
    /// Handle initialize event.
    /// </summary>
    public void OnInitialize(object sender, EventArgs _)
    {
        Engine = sender as ITradingEngine;
        logger = Engine.CreateLogger(this.GetType().Name);
    }
}