using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Me
{
    public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetCurrentUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<CurrentUserResponse> Handle(
            GetCurrentUserQuery request,
            CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);

            if (user is null)
                throw new UnauthorizedException();

            return new CurrentUserResponse(user.Id, user.Email);
        }
    }
}
