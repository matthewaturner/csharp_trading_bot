// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Exceptions;

public class InvalidOrderException : Exception
{
    public InvalidOrderException()
        : base()
    { }

    public InvalidOrderException(string message)
        : base(message)
    { }
}
