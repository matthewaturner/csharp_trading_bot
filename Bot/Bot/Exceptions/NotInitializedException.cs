// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Exceptions;

public class NotInitializedException : Exception
{
    public NotInitializedException()
        : base()
    { }

    public NotInitializedException(string message)
        : base(message)
    { }
}
