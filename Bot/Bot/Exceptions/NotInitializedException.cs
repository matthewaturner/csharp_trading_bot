using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Exceptions
{
    public class NotInitializedException : Exception
    {
        public NotHydratedException()
            : base()
        { }

        public NotHydratedException(string message)
            : base(message)
        { }
    }
}
