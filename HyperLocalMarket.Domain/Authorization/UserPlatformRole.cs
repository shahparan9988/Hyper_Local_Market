using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Authorization
{
    public sealed class UserPlatformRole : AggregateRoot
    {
        private UserPlatformRole()
        {
        }

        private UserPlatformRole(
            Guid userId,
            Guid platformRoleId,
            Guid assignedByUserId,
            DateTime utcNow)
        {
            Guard.NotEmpty(userId, nameof(userId));
            Guard.NotEmpty(platformRoleId, nameof(platformRoleId));
            Guard.NotEmpty(assignedByUserId, nameof(assignedByUserId));
            Guard.Utc(utcNow, nameof(utcNow));

            Id = Guid.NewGuid();
            UserId = userId;
            PlatformRoleId = platformRoleId;
            AssignedByUserId = assignedByUserId;
            AssignedAtUtc = utcNow;
        }

        public Guid UserId { get; private set; }
        public Guid PlatformRoleId { get; private set; }
        public Guid AssignedByUserId { get; private set; }
        public DateTime AssignedAtUtc { get; private set; }
        public Guid? RevokedByUserId { get; private set; }
        public DateTime? RevokedAtUtc { get; private set; }

        public bool IsActive => RevokedAtUtc is null;

        public static UserPlatformRole Assign(
            Guid userId,
            Guid platformRoleId,
            Guid assignedByUserId,
            DateTime utcNow)
        {
            return new UserPlatformRole(
                userId,
                platformRoleId,
                assignedByUserId,
                utcNow);
        }

        public void Revoke(
            Guid revokedByUserId,
            DateTime utcNow)
        {
            Guard.NotEmpty(revokedByUserId, nameof(revokedByUserId));
            Guard.Utc(utcNow, nameof(utcNow));

            if (!IsActive)
            {
                return;
            }

            if (utcNow < AssignedAtUtc)
            {
                throw new DomainException(
                    "A role cannot be revoked before it was assigned.");
            }

            RevokedByUserId = revokedByUserId;
            RevokedAtUtc = utcNow;
        }
    }
}
