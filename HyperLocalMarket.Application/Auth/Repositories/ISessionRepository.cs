using HyperLocalMarket.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Repositories
{
    public interface ISessionRepository
    {
        Task<Session?> GetActiveSessionByTokenHashAsync(string tokenHash, CancellationToken ct);
    }
}
