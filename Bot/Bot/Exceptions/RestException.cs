// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Exceptions;

public class RestException : Exception
{
    public RestException()
        : base()
    { }

    public RestException(string message)
        : base(message)
    { }
}
