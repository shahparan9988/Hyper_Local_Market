using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Exceptions
{
    public sealed class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }
        public ValidationException(IReadOnlyDictionary<string, string[]> error)
            : base("validation Failed") 
        {
            Errors = error;
        }
    }
}
