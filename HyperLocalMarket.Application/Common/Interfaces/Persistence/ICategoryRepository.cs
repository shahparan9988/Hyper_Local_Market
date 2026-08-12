using HyperLocalMarket.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Common.Interfaces.Persistence
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
    }
}
