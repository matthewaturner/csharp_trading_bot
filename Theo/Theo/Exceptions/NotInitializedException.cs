using System;

namespace Theo.Exceptions
{
    public class NotInitializedException : Exception
    {
        public NotInitializedException()
            : base()
        { }

        public NotInitializedException(string message)
            : base(message)
        { }
    }
}
