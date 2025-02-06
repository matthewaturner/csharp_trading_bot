
namespace Theo.Engine.Events
{
    public interface IInitialize
    {
        /// <summary>
        /// Initialize the object, called on startup.
        /// </summary>
        /// <param name="engine">Engine object.</param>
        public void Initialize(ITradingEngine engine);
    }
}