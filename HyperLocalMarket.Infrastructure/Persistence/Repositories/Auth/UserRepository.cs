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
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User?> GetByEmailOrPhoneWithSessionsAsync(
            string email, string phone,
            CancellationToken ct)
        {
            return _dbContext.Users
                .Include(x => x.Sessions)
                .FirstOrDefaultAsync(x => x.Email == email || x.Phone == phone, ct);
        }

        public Task<User?> GetByIdWithSessionsAsync(
            Guid userId,
            CancellationToken ct)
        {
            return _dbContext.Users
                .Include(x => x.Sessions)
                .FirstOrDefaultAsync(x => x.Id == userId, ct);
        }

        public Task<User?> GetByIdAsync(
            Guid userId,
            CancellationToken ct)
        {
            return _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _dbContext.Users.AddAsync(user, ct);
        }
    }

}
