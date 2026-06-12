using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Domain.Users;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Register
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterResponse> Handle(
            RegisterCommand request,
            CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var existingUser = await _userRepository.GetByEmailWithSessionsAsync(
                email, ct);
            if (existingUser is not null)
                throw new ConflictException("Email is already registered.");
            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = new User(email, passwordHash);
            await _userRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return new RegisterResponse(user.Id, user.Email);
        }

    }
}
