using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Services
{
    public interface IPlatformPermissionChecker
    {
        Task<bool> HasPermissionAsync(
            Guid userId,
            string permission,
            CancellationToken cancellationToken = default);
    }
}
