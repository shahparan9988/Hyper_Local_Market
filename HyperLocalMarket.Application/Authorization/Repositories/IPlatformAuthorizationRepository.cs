using HyperLocalMarket.Domain.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Repositories
{
    public interface IPlatformAuthorizationRepository
    {
        Task<bool> UserExistsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<bool> UserOwnsAnyStoreAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<bool> UserOwnsStoreAsync(
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken = default);

        Task<PlatformRole?> GetRoleByCodeAsync(
            string roleCode,
            CancellationToken cancellationToken = default);

        Task<UserPlatformRole?> GetActiveAssignmentAsync(
            Guid userId,
            Guid platformRoleId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> GetRoleCodesAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> GetPermissionCodesAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<int> CountActiveAssignmentsForRoleAsync(
            Guid platformRoleId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            UserPlatformRole assignment,
            CancellationToken cancellationToken = default);
    }
}
