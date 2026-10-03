using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.ProcessProductImage
{
    public sealed record ProcessProductImageCommand(
        string OriginalObjectKey) : IRequest;
}
