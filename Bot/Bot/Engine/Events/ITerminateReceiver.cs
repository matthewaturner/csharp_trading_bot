
namespace Bot.Engine.Events
{
    interface ITerminateReceiver
    {
        /// <summary>
        /// Function to execute when the run is over.
        /// </summary>
        public void OnTerminate();
    }
}
