using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus
{
    public sealed class GetStoreSetupStatusQueryValidator
    : AbstractValidator<GetStoreSetupStatusQuery>
    {
        public GetStoreSetupStatusQueryValidator()
        {
            RuleFor(x => x.StoreId)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
