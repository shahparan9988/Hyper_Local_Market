using HyperLocalMarket.Application.Stores.Queries.GetMyStores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Queries.Stores
{
    public sealed class MyStoresReader : IMyStoresReader
    {
        private readonly AppDbContext _dbContext;

        public MyStoresReader(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<MyStoreDto>> GetAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var stores = await _dbContext.Stores
                .AsNoTracking()
                .Where(store => store.UserId == userId)
                .OrderByDescending(store => store.CreatedAtUtc)
                .ThenBy(store => store.Id)
                .Select(store => new
                {
                    store.Id,
                    store.Name,
                    store.Status,

                    AddressLine1 =
                        store.Location.Address.AddressLine1,

                    Locality =
                        store.Location.Address.Locality,

                    Region =
                        store.Location.Address.Region
                })
                .ToListAsync(cancellationToken);

            return stores
                .Select(store => new MyStoreDto(
                    Id: store.Id,
                    Name: store.Name,
                    Status: store.Status.ToString(),
                    Location: new MyStoreLocationDto(
                        Address: new MyStoreAddressDto(
                            AddressLine1: store.AddressLine1,
                            Locality: store.Locality,
                            Region: store.Region))))
                .ToList();
        }
    }
}
