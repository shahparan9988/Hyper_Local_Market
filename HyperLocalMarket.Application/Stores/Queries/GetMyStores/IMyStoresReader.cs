using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetMyStores
{
    public interface IMyStoresReader
    {
        Task<IReadOnlyList<MyStoreDto>> GetAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
