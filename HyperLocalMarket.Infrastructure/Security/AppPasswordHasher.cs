using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Security
{
    public sealed class AppPasswordHasher : IPasswordHasher
    {
        private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _passwordHasher = new();

        public string Hash(string password)
        {
            return _passwordHasher.HashPassword(new object(), password);
        }

        public bool Verify(string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

            return result is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
