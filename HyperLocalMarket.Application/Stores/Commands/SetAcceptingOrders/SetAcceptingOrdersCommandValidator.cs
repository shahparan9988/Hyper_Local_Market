using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.SetAcceptingOrders
{
    public sealed class SetAcceptingOrdersCommandValidator
        : AbstractValidator<SetAcceptingOrdersCommand>
    {
        public SetAcceptingOrdersCommandValidator()
        {
            RuleFor(command => command.StoreId).NotEmpty();
            RuleFor(command => command.UserId).NotEmpty();
        }
    }

}
