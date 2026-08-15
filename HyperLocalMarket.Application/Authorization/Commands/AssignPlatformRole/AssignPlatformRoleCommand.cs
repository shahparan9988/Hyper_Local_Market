using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Commands.AssignPlatformRole
{
    public sealed record AssignPlatformRoleCommand(
        Guid ActorUserId,
        Guid TargetUserId,
        string RoleCode) : IRequest;
}
