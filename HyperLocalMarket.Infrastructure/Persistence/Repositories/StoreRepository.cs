using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _dbContext;
        public StoreRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Store store, CancellationToken cancellationToken = default)
        {
            await _dbContext.Stores.AddAsync(store, cancellationToken);
        }

        public Task<Store?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Stores.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public Task<List<Store>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.Stores.ToListAsync(cancellationToken);
        }
        public Task<bool> ExistByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return _dbContext.Stores.AnyAsync(s => s.Name == name, cancellationToken);
        }
    }
}
