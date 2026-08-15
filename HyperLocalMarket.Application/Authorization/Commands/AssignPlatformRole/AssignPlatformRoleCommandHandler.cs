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

namespace HyperLocalMarket.Application.Authorization.Commands.AssignPlatformRole
{
    public sealed class AssignPlatformRoleCommandHandler : IRequestHandler<AssignPlatformRoleCommand>
    {
        private readonly IPlatformAuthorizationRepository _platformAuthorizationRepository;
        private readonly IPlatformPermissionChecker _permissionChecker;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public AssignPlatformRoleCommandHandler(
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
            AssignPlatformRoleCommand command,
            CancellationToken cancellationToken)
        {
            var canAssignRoles = await _permissionChecker.HasPermissionAsync(
                command.ActorUserId,
                PlatformPermissionCodes.RolesAssign,
                cancellationToken);

            if (!canAssignRoles)
            {
                throw new ForbiddenException(
                    "You do not have permission to assign platform roles.");
            }

            if (!await _platformAuthorizationRepository.UserExistsAsync(
                    command.TargetUserId,
                    cancellationToken))
            {
                throw new NotFoundException("Target user was not found.");
            }

            var role = await _platformAuthorizationRepository.GetRoleByCodeAsync(
                command.RoleCode,
                cancellationToken);

            if (role is null)
            {
                throw new NotFoundException(
                    $"Platform role '{command.RoleCode}' was not found.");
            }

            var currentAssignment =
                await _platformAuthorizationRepository.GetActiveAssignmentAsync(
                    command.TargetUserId,
                    role.Id,
                    cancellationToken);

            // PUT semantics: assigning an already-active role is a successful no-op.
            if (currentAssignment is not null)
            {
                return;
            }

            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            var assignment = UserPlatformRole.Assign(
                command.TargetUserId,
                role.Id,
                command.ActorUserId,
                utcNow);

            await _platformAuthorizationRepository.AddAsync(
                assignment,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
