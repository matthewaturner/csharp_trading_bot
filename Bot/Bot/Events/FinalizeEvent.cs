
namespace Bot.Events;

/// <summary>
/// The event to raise.
/// </summary>
public class FinalizeEvent()
{
}

/// <summary>
/// Defines the method implemented by event receivers.
/// </summary>
public interface IFinalizeReceiver
{
    public void OnFinalize(object sender, FinalizeEvent e);
}