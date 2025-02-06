using System;

namespace Theo.Exceptions
{
    public class InvalidOrderException : Exception
    {
        public InvalidOrderException()
            : base()
        { }

        public InvalidOrderException(string message)
            : base(message)
        { }
    }
}
