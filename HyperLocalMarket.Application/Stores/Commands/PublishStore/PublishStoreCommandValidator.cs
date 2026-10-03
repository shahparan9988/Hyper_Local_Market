using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.PublishStore
{
    public sealed class PublishStoreCommandValidator : AbstractValidator<PublishStoreCommand>
    {
        public PublishStoreCommandValidator()
        {
            RuleFor(command => command.StoreId).NotEmpty();
            RuleFor(command => command.UserId).NotEmpty();
        }
    }
}
