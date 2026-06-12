using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Exceptions
{
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Unauthorized") : base(message) { }
    }
}
