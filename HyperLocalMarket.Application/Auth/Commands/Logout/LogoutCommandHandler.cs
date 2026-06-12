using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Logout
{
    public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            LogoutCommand request,
            CancellationToken ct)
        {
            var user = await _userRepository.GetByIdWithSessionsAsync(
                request.UserId,
                ct);

            if (user is null)
                throw new UnauthorizedException();

            user.RevokeSession(request.SessionId);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
