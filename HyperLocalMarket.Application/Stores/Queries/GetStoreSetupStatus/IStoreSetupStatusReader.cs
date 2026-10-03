using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus
{
    public interface IStoreSetupStatusReader
    {
        Task<StoreSetupStatusData?> GetAsync(
            Guid storeId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
