using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message = "Not Found") : base(message) { }
    }
}
