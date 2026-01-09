using HyperLocalMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Common.Interfaces.Persistence
{
    public interface IStoreRepository
    {
        Task AddAsync(Store store, CancellationToken cancellationToken = default);
        Task<Store?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Store>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistByNameAsync(string name, CancellationToken cancellationToken = default);

    }
}
