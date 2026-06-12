using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Register
{
    public sealed record RegisterCommand(
        string Email,
        string Password,
        string? IpAddress,
        string? UserAgent
    ) : IRequest<RegisterResponse>;
}
