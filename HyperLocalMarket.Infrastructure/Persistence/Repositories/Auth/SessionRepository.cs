using HyperLocalMarket.Application.Auth.Repositories;
using HyperLocalMarket.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories.Auth
{
    public sealed class SessionRepository : ISessionRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly TimeProvider _timeProvider;


        public SessionRepository(AppDbContext dbContext, TimeProvider timeProvider)
        {
            _dbContext = dbContext;
            _timeProvider = timeProvider;
        }

        public async Task<Session?> GetActiveSessionByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            return await(
                    from session in _dbContext.Sessions
                    join user in _dbContext.Users
                        on session.UserId equals user.Id
                    where session.TokenHash == tokenHash &&
                          session.IsActive &&
                          session.ExpiresAtUtc > utcNow &&
                          user.IsActive
                    select session)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }

}
