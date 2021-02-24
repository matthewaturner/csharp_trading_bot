using Bot.Brokers;
using Bot.Engine;

namespace Bot.Analyzers
{
    public interface IAnalyzer
    {
        /// <summary>
        /// Initializes the data source with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        void Initialize(ITradingEngine engine, string[] args);

        /// <summary>
        /// Writes output as a string.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
