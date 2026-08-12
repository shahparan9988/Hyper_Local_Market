using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Users.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Users
{
    public sealed class User : AggregateRoot
    {
        private readonly List<Session> _sessions = new();
        public string Phone { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

        private User() { }

        public User(string email, string phone, string passwordHash)
        {
            Id = Guid.NewGuid();
            Email = email.ToLowerInvariant();
            Phone = phone;
            PasswordHash = passwordHash;
            IsActive = true;
        }

        public Session Login(
            string deviceKey,
            string tokenHash,
            DateTime expiresAtUtc,
            string? ipAddress,
            string? userAgent)
        {
            if (!IsActive)
                throw new InvalidOperationException("User account is inactive.");

            var existingActiveSession = _sessions.FirstOrDefault(x =>
                x.DeviceKey == deviceKey &&
                x.IsActive);

            existingActiveSession?.Revoke();

            var session = new Session(
                userId: Id,
                deviceKey: deviceKey,
                tokenHash: tokenHash,
                expiresAtUtc: expiresAtUtc,
                ipAddress: ipAddress,
                userAgent: userAgent);

            _sessions.Add(session);

            AddDomainEvent(new UserLoggedInDomainEvent(
                UserId: Id,
                SessionId: session.Id,
                OccurredAtUtc: DateTime.UtcNow));

            return session;
        }

        public void RevokeSession(Guid sessionId)
        {
            var session = _sessions.FirstOrDefault(x => x.Id == sessionId);

            if (session is null)
                throw new InvalidOperationException("Session does not belong to this user.");

            session.Revoke();

            AddDomainEvent(new UserSessionRevokedDomainEvent(
                UserId: Id,
                SessionId: session.Id,
                OccurredAtUtc: DateTime.UtcNow));
        }

        public void RevokeAllSessions()
        {
            foreach (var session in _sessions.Where(x => x.IsActive))
            {
                session.Revoke();

                AddDomainEvent(new UserSessionRevokedDomainEvent(
                    UserId: Id,
                    SessionId: session.Id,
                    OccurredAtUtc: DateTime.UtcNow));
            }
        }
    }
}
