using FluentValidation;
using HyperLocalMarket.Domain.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Commands.AssignPlatformRole
{
    public sealed class AssignPlatformRoleCommandValidator : AbstractValidator<AssignPlatformRoleCommand>
    {
        public AssignPlatformRoleCommandValidator()
        {
            RuleFor(command => command.ActorUserId)
                .NotEmpty();

            RuleFor(command => command.TargetUserId)
                .NotEmpty();

            RuleFor(command => command.RoleCode)
                .NotEmpty()
                .MaximumLength(PlatformRole.CodeMaxLength);
        }
    }
}
