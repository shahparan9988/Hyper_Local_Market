using FluentValidation;
using HyperLocalMarket.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.StoreId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Product.NameMaxLength);

            RuleFor(x => x.Description)
                .MaximumLength(Product.DescriptionMaxLength)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.BrandName)
                .MaximumLength(Product.BrandMaxLength)
                .When(x => !string.IsNullOrWhiteSpace(x.BrandName));

            //RuleFor(x => x.CategoryId)
            //    .NotEqual(Guid.Empty)
            //    .When(x => x.CategoryId.HasValue);
        }
    }
}
