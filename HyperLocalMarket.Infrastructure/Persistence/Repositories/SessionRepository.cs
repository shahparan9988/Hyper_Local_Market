using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories
{
    public sealed class SessionRepository : ISessionRepository
    {
        private readonly AppDbContext _dbContext;

        public SessionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Session?> GetActiveSessionByTokenHashAsync(
            string tokenHash,
            CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            return _dbContext.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    x.RevokedAtUtc == null &&
                    x.ExpiresAtUtc > now,
                    ct);
        }
    }

}
