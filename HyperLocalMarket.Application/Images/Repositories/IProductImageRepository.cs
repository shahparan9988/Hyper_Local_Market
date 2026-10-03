using HyperLocalMarket.Domain.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Repositories
{
    public interface IProductImageWork : IAsyncDisposable
    {
        ProductImageAsset? Asset { get; }
        Task CommitAsync(CancellationToken ct);
    }
    public interface IProductImageRepository
    {
        void Add(ProductImageAsset asset);
        Task<ProductImageAsset?> GetAsync(Guid storeId, Guid userId, Guid uploadId, CancellationToken ct);
        Task<IProductImageWork> BeginWorkAsync(string originalKey, CancellationToken ct);
    }
}
