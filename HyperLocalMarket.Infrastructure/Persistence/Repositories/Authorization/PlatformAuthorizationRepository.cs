using HyperLocalMarket.Application.Authorization.Repositories;
using HyperLocalMarket.Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories.Authorization
{
    public sealed class PlatformAuthorizationRepository
        : IPlatformAuthorizationRepository
    {
        private readonly AppDbContext _dbContext;

        public PlatformAuthorizationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> UserExistsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user => user.Id == userId,
                    cancellationToken);
        }

        public Task<bool> UserOwnsAnyStoreAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.Stores
                .AsNoTracking()
                .AnyAsync(
                    store => store.UserId == userId,
                    cancellationToken);
        }

        public Task<bool> UserOwnsStoreAsync(
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.Stores
                .AsNoTracking()
                .AnyAsync(
                    store => store.UserId == userId && store.Id == storeId,
                    cancellationToken);
        }

        public Task<PlatformRole?> GetRoleByCodeAsync(
            string roleCode,
            CancellationToken cancellationToken = default)
        {
            var normalizedRoleCode = roleCode
                .Trim()
                .ToLowerInvariant();

            return _dbContext.PlatformRoles
                .SingleOrDefaultAsync(
                    role => role.Code == normalizedRoleCode,
                    cancellationToken);
        }

        public Task<UserPlatformRole?> GetActiveAssignmentAsync(
            Guid userId,
            Guid platformRoleId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.UserPlatformRoles
                .SingleOrDefaultAsync(
                    assignment =>
                        assignment.UserId == userId &&
                        assignment.PlatformRoleId == platformRoleId &&
                        assignment.RevokedAtUtc == null,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetRoleCodesAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await (
                    from assignment in _dbContext.UserPlatformRoles.AsNoTracking()
                    join role in _dbContext.PlatformRoles.AsNoTracking()
                        on assignment.PlatformRoleId equals role.Id
                    where assignment.UserId == userId &&
                          assignment.RevokedAtUtc == null
                    orderby role.Code
                    select role.Code)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetPermissionCodesAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await (
                    from assignment in _dbContext.UserPlatformRoles.AsNoTracking()
                    join rolePermission in
                        _dbContext.PlatformRolePermissions.AsNoTracking()
                        on assignment.PlatformRoleId equals
                        rolePermission.PlatformRoleId
                    join permission in
                        _dbContext.PlatformPermissions.AsNoTracking()
                        on rolePermission.PlatformPermissionId equals
                        permission.Id
                    where assignment.UserId == userId &&
                          assignment.RevokedAtUtc == null
                    orderby permission.Code
                    select permission.Code)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountActiveAssignmentsForRoleAsync(
            Guid platformRoleId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.UserPlatformRoles
                .AsNoTracking()
                .CountAsync(
                    assignment =>
                        assignment.PlatformRoleId == platformRoleId &&
                        assignment.RevokedAtUtc == null,
                    cancellationToken);
        }

        public Task AddAsync(
            UserPlatformRole assignment,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.UserPlatformRoles
                .AddAsync(assignment, cancellationToken)
                .AsTask();
        }
    }
}
