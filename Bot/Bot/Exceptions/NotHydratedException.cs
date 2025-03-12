// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Exceptions;

public class NotHydratedException : Exception
{
    public NotHydratedException()
        : base()
    { }

    public NotHydratedException(string message)
        : base(message)
    { }
}
