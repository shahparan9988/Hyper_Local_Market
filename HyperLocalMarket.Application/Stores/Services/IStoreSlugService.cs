using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Services
{
    public interface IStoreSlugService
    {
        string GenerateUnique(string storeName);
    }
}
