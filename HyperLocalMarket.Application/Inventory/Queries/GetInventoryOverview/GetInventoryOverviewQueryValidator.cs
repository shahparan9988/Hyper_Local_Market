using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Inventory.Queries.GetInventoryOverview
{
    public sealed class GetInventoryOverviewQueryValidator
            : AbstractValidator<GetInventoryOverviewQuery>
    {
        public GetInventoryOverviewQueryValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Filter).NotNull();

            When(x => x.Filter is not null, () =>
            {
                RuleFor(x => x.Filter.Search)
                    .MaximumLength(150);

                RuleFor(x => x.Filter.CategoryId)
                    .Must(id => !id.HasValue || id.Value != Guid.Empty)
                    .WithMessage("Choose a valid store category.");

                RuleFor(x => x.Filter.StockStatus)
                    .Must(status => !status.HasValue ||
                        Enum.IsDefined(status.Value))
                    .WithMessage("Choose a valid stock status.");

                RuleFor(x => x.Filter.Page)
                    .InclusiveBetween(1, 1_000_000);

                RuleFor(x => x.Filter.PageSize)
                    .InclusiveBetween(1, 100);
            });
        }
    }
}
