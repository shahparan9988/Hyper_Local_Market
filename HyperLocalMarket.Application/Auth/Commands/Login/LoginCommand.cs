using HyperLocalMarket.Shared.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Login
{
    public sealed record LoginCommand
    (
        string EmailOrPhone,
        string Password,
        string DeviceKey,
        string? IpAddress,
        string? UserAgent
    ) : IRequest<Result<LoginResponse>>;
}
