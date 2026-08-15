using HyperLocalMarket.Application.Authorization.Repositories;
using HyperLocalMarket.Application.Authorization.Services;
using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Domain.Authorization;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Commands.RevokePlatformRole
{
    public sealed class RevokePlatformRoleCommandHandler
        : IRequestHandler<RevokePlatformRoleCommand>
    {
        private readonly IPlatformAuthorizationRepository _platformAuthorizationRepository;
        private readonly IPlatformPermissionChecker _permissionChecker;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RevokePlatformRoleCommandHandler(
            IPlatformAuthorizationRepository platformAuthorizationRepository,
            IPlatformPermissionChecker permissionChecker,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _platformAuthorizationRepository = platformAuthorizationRepository;
            _permissionChecker = permissionChecker;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task Handle(
            RevokePlatformRoleCommand command,
            CancellationToken cancellationToken)
        {
            var canAssignRoles = await _permissionChecker.HasPermissionAsync(
                command.ActorUserId,
                PlatformPermissionCodes.RolesAssign,
                cancellationToken);

            if (!canAssignRoles)
            {
                throw new ForbiddenException(
                    "You do not have permission to revoke platform roles.");
            }

            var role = await _platformAuthorizationRepository.GetRoleByCodeAsync(
                command.RoleCode,
                cancellationToken);

            if (role is null)
            {
                throw new NotFoundException(
                    $"Platform role '{command.RoleCode}' was not found.");
            }

            var assignment = await _platformAuthorizationRepository.GetActiveAssignmentAsync(
                command.TargetUserId,
                role.Id,
                cancellationToken);

            // DELETE semantics: an already-absent assignment is a successful no-op.
            if (assignment is null)
            {
                return;
            }

            if (role.Code == PlatformRoleCodes.PlatformAdmin)
            {
                var activeAdministratorCount =
                    await _platformAuthorizationRepository.CountActiveAssignmentsForRoleAsync(
                        role.Id,
                        cancellationToken);

                if (activeAdministratorCount <= 1)
                {
                    throw new ConflictException(
                        "The final platform administrator cannot be revoked.");
                }
            }

            assignment.Revoke(
                command.ActorUserId,
                _timeProvider.GetUtcNow().UtcDateTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
