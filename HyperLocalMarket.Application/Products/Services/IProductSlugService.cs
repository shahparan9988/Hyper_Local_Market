using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Services
{
    public interface IProductSlugService
    {
        Task<string> GenerateUniqueSlugAsync(Guid storeId, string productName, CancellationToken cancellationToken);
    }
}
