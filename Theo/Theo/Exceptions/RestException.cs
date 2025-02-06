using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theo.Exceptions
{
    public class RestException : Exception
    {
        public RestException()
            : base()
        { }

        public RestException(string message)
            : base(message)
        { }
    }
}
