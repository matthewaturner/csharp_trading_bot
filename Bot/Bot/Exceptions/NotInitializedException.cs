using System;

namespace Bot.Exceptions
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
