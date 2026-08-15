using HyperLocalMarket.Application.Auth.Repositories;
using HyperLocalMarket.Application.Auth.Services;
using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Common.Security;
using HyperLocalMarket.Shared.Exceptions;
using HyperLocalMarket.Shared.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LoginResponse>> Handle(
            LoginCommand request,
            CancellationToken ct)
        {
            var user = await _userRepository.GetByEmailOrPhoneWithSessionsAsync(
                request.EmailOrPhone.ToLowerInvariant(), request.EmailOrPhone.ToLowerInvariant(),
                ct);

            if (user is null)
                throw new UnauthorizedException("Invalid email or password.");

            var validPassword = _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

            if (!validPassword)
                throw new UnauthorizedException("Invalid email or password.");

            var rawToken = SessionTokenGenerator.GenerateRawToken();
            var tokenHash = SessionTokenGenerator.HashToken(rawToken);
            var expiresAtUtc = DateTime.UtcNow.AddDays(30);

            var session = user.Login(
                request.DeviceKey,
                tokenHash,
                expiresAtUtc,
                request.IpAddress,
                request.UserAgent);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<LoginResponse>.Success(new LoginResponse(
                user.Id,
                session.Id,
                rawToken,
                expiresAtUtc));
        }

    }
}
