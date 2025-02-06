using System;

namespace Theo.Exceptions
{
    public class BadDataException : Exception
    {
        public BadDataException()
        : base()
        { }

        public BadDataException(string message)
            : base(message)
        { }
    }
}
