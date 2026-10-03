using FluentValidation;
using HyperLocalMarket.Application.Products.Dtos;
using HyperLocalMarket.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.SaveProduct
{
    public sealed class SaveProductCommandValidator : AbstractValidator<SaveProductCommand>
    {
        public SaveProductCommandValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty(); RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.IdempotencyKey).NotNull().NotEqual(Guid.Empty).When(x => !x.ProductId.HasValue);
            RuleFor(x => x.ProductId).NotEqual(Guid.Empty).When(x => x.ProductId.HasValue);
            RuleFor(x => x.Input).NotNull().SetValidator(new SaveProductInputValidator());
        }
    }
    public sealed class SaveProductInputValidator : AbstractValidator<SaveProductInput>
    {
        public SaveProductInputValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(5000);
            RuleFor(x => x.Type).Must(x => x is "Product" or "Service").WithMessage("Choose Product or Service.");
            RuleFor(x => x.Status).Must(x => x is "Draft" or "Published").WithMessage("Choose Draft or Published.");
            RuleFor(x => x.CurrencyCode).Must(x => x is "BDT" or "AUD");
            RuleFor(x => x.ManualAvailability).Must(x => x is "Available" or "Unavailable");
            RuleFor(x => x.StoreCategoryId).NotEqual(Guid.Empty).When(x => x.StoreCategoryId.HasValue);
            RuleFor(x => x.MarketplaceCategoryId).NotEqual(Guid.Empty).When(x => x.MarketplaceCategoryId.HasValue);
            RuleFor(x => x.VariantOptionName).MaximumLength(50);
            RuleFor(x => x.Options).NotNull()
                .Must(x => x is { Count: <= Product.MaximumOptionCount })
                .WithMessage("Reload the product editor to supply structured options.");
            When(x => x.Options is not null, () => RuleForEach(x => x.Options).NotNull().ChildRules(option =>
            {
                option.RuleFor(x => x.Id).NotEmpty();
                option.RuleFor(x => x.Name).NotEmpty().MaximumLength(ProductOption.NameMaxLength);
                option.RuleFor(x => x.Values).NotNull()
                    .Must(x => x is { Count: > 0 and <= ProductOptionSet.MaximumValuesPerOption });
                option.When(x => x.Values is not null, () => option.RuleForEach(x => x.Values).NotNull().ChildRules(value =>
                {
                    value.RuleFor(x => x.Id).NotEmpty();
                    value.RuleFor(x => x.Value).NotEmpty().MaximumLength(ProductOptionValue.ValueMaxLength);
                }));
            }));
            RuleFor(x => x.Variants).NotNull().Must(x => x is { Count: > 0 and <= ProductOptionSet.MaximumCombinations }).WithMessage("Supply 1–1000 variants.");
            RuleFor(x => x.ImageAssetIds).NotNull().Must(x => x is { Count: <= 8 } && x.All(id => id != Guid.Empty) && x.Distinct().Count() == x.Count);
            RuleFor(x => x.DeliveryOptionIds).NotNull().Must(x => x is { Count: <= 50 } && x.All(id => id != Guid.Empty) && x.Distinct().Count() == x.Count);
            When(x => x.Variants is not null, () => RuleForEach(x => x.Variants).NotNull().ChildRules(v =>
            {
                v.RuleFor(x => x.Id).NotEmpty(); v.RuleFor(x => x.Name).MaximumLength(ProductVariant.DisplayNameMaxLength);
                v.RuleFor(x => x.Selections).NotNull()
                    .Must(x => x is { Count: <= Product.MaximumOptionCount } &&
                        x.All(s => s is not null && s.OptionId != Guid.Empty && s.ValueId != Guid.Empty));
                v.RuleFor(x => x.Sku).MaximumLength(100);
                v.RuleFor(x => x.Price).Must(Money).WithMessage("Invalid selling price.");
                v.RuleFor(x => x.CompareAtPrice).Must(Money).WithMessage("Invalid original price.");
            }));
        }
        private static bool Money(decimal? value) => !value.HasValue || value >= 0 && value <= 999999999.99m && decimal.Round(value.Value, 2) == value;
    }


}
