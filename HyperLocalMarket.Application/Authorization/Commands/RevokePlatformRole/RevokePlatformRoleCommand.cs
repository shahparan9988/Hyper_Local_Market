using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Commands.RevokePlatformRole
{
    public sealed record RevokePlatformRoleCommand(
        Guid ActorUserId,
        Guid TargetUserId,
        string RoleCode) : IRequest;
}
