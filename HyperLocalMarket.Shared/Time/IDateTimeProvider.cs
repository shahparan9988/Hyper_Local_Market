using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Time
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
