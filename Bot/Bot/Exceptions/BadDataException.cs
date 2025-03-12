// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Exceptions;

public class BadDataException : Exception
{
    public BadDataException()
    : base()
    { }

    public BadDataException(string message)
        : base(message)
    { }
}
