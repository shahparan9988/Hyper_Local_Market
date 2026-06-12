using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Results
{
    public sealed record ValidationError(
        string PropertyName,
        string Message
    );
}
