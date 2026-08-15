using HyperLocalMarket.Application.Authorization.Repositories;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Queries.GetMyAccess
{
    public sealed class GetMyAccessQueryHandler : IRequestHandler<GetMyAccessQuery, MyAccessDto>
    {
        private readonly IPlatformAuthorizationRepository _repository;

        public GetMyAccessQueryHandler(
            IPlatformAuthorizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<MyAccessDto> Handle(
            GetMyAccessQuery query,
            CancellationToken cancellationToken)
        {
            if (!await _repository.UserExistsAsync(
                    query.UserId,
                    cancellationToken))
            {
                throw new NotFoundException("User was not found.");
            }

            // Run sequentially because all queries use the same scoped DbContext.
            var isSeller = await _repository.UserOwnsAnyStoreAsync(
                query.UserId,
                cancellationToken);

            var roles = await _repository.GetRoleCodesAsync(
                query.UserId,
                cancellationToken);

            var permissions = await _repository.GetPermissionCodesAsync(
                query.UserId,
                cancellationToken);

            return new MyAccessDto(
                UserId: query.UserId,
                IsBuyer: true,
                IsSeller: isSeller,
                PlatformRoles: roles,
                PlatformPermissions: permissions);
        }
    }
}
