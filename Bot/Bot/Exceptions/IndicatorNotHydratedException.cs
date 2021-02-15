using System;

namespace Bot.Exceptions
{
    public class IndicatorNotHydratedException : Exception
    {
        public IndicatorNotHydratedException()
            : base()
        { }

        public IndicatorNotHydratedException(string message)
            : base(message)
        { }
    }
}
