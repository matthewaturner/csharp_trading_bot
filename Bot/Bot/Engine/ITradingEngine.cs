using System.Threading.Tasks;

namespace Bot.Trading
{
    public interface ITradingEngine
    {
        /// <summary>
        /// Runs the trading engine.
        /// </summary>
        /// <returns></returns>
        public Task RunAsync();

    }
}
