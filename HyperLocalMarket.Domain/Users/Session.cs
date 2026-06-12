using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Users
{
    public sealed class Session : Entity
    {
        public Guid UserId { get; private set; }

        public string DeviceKey { get; private set; } = default!;
        public string TokenHash { get; private set; } = default!;
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public DateTime? RevokedAtUtc { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }

        public bool IsRevoked => RevokedAtUtc is not null;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsActive => !IsRevoked && !IsExpired;

        private Session() { }

        internal Session(
            Guid userId,
            string deviceKey,
            string tokenHash,
            DateTime expiresAtUtc,
            string? ipAddress,
            string? userAgent)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            DeviceKey = deviceKey;
            TokenHash = tokenHash;
            CreatedAtUtc = DateTime.UtcNow;
            ExpiresAtUtc = expiresAtUtc;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }

        internal void Revoke()
        {
            if (IsRevoked)
                return;

            RevokedAtUtc = DateTime.UtcNow;
        }
    }
}
