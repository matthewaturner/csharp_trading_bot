using System;

namespace Theo.Exceptions
{
    public class NotHydratedException : Exception
    {
        public NotHydratedException()
            : base()
        { }

        public NotHydratedException(string message)
            : base(message)
        { }
    }
}
