
namespace Bot.Events;

public interface IEventReceiver<T>
{
    public void OnEvent(object sender, T e);
}
