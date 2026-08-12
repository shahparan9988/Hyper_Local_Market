using HyperLocalMarket.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailOrPhoneWithSessionsAsync(string email, string phone, CancellationToken ct);
        Task<User?> GetByIdWithSessionsAsync(Guid userId, CancellationToken ct);
        Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
    }
}
