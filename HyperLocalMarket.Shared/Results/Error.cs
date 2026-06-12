using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Results
{
    public sealed record Error(string code, string message)
    {
        public static readonly Error None = new Error(string.Empty, string.Empty);

        public static Error Create(string code, string message) 
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Error code cannot be empty.", nameof(code));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Error message cannot be empty.", nameof(message));

            return new Error(code, message);
        }
    }
}
