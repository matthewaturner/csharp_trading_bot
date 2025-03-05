using System;

namespace Bot.Events;

/// <summary>
/// Defines the method implemented by event receivers.
/// </summary>
public interface IFinalizeReceiver
{
    public void OnFinalize(object sender, EventArgs _);
}