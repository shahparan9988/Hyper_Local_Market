using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Common.Abstractions
{
    public interface ISlugGenerator
    {
        string Generate(string value, int maxLength);
    }
}
