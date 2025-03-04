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
