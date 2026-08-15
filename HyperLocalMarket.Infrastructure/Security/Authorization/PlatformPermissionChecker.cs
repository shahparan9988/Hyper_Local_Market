using HyperLocalMarket.Application.Authorization.Repositories;
using HyperLocalMarket.Application.Authorization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Security.Authorization
{
    public sealed class PlatformPermissionChecker
        : IPlatformPermissionChecker
    {
        private readonly IPlatformAuthorizationRepository _repository;

        private Guid? _loadedUserId;
        private HashSet<string>? _loadedPermissions;

        public PlatformPermissionChecker(
            IPlatformAuthorizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> HasPermissionAsync(
            Guid userId,
            string permission,
            CancellationToken cancellationToken = default)
        {
            if (_loadedPermissions is null ||
                _loadedUserId != userId)
            {
                var permissions =
                    await _repository.GetPermissionCodesAsync(
                        userId,
                        cancellationToken);

                _loadedPermissions = new HashSet<string>(
                    permissions,
                    StringComparer.Ordinal);

                _loadedUserId = userId;
            }

            return _loadedPermissions.Contains(permission);
        }
    }
}
