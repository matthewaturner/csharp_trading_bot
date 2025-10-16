
using System;

namespace Bot.Events;

public interface IInitializeReceiver
{
    /// <summary>
    /// Initialize the object, called on startup.
    /// </summary>
    /// <param name="sender">The engine.</param>
    public void OnInitialize(object sender, EventArgs _);
}