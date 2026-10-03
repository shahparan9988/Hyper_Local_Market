using FluentValidation;
using HyperLocalMarket.Application.Stores.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.DeliveryOptions
{
    public sealed class DeliveryOptionInputValidator : AbstractValidator<DeliveryOptionInput>
    {
        public DeliveryOptionInputValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.CoverageDescription).NotEmpty().MaximumLength(300);
            RuleFor(x => x.EstimatedTimeDescription).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Conditions).MaximumLength(1000);
            RuleFor(x => x.CurrencyCode).Must(x => x is "BDT" or "AUD")
                .WithMessage("Choose BDT or AUD.");
            RuleFor(x => x.FeeType).Must(x => x is "Fixed" or "StartingFrom" or "ContactSeller")
                .WithMessage("Choose Fixed, StartingFrom or ContactSeller.");
            When(x => x.FeeType == "ContactSeller", () =>
                RuleFor(x => x.FeeAmount).Null());
            When(x => x.FeeType is "Fixed" or "StartingFrom", () =>
                RuleFor(x => x.FeeAmount).Must(x => x.HasValue && x.Value >= 0 &&
                    x.Value <= 999999999.99m && decimal.Round(x.Value, 2) == x.Value)
                    .WithMessage("Enter a non-negative amount with at most two decimal places."));
        }
    }

    public sealed class CreateDeliveryOptionValidator : AbstractValidator<CreateDeliveryOptionCommand>
    {
        public CreateDeliveryOptionValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Input).NotNull().SetValidator(new DeliveryOptionInputValidator());
        }
    }
    public sealed class UpdateDeliveryOptionValidator : AbstractValidator<UpdateDeliveryOptionCommand>
    {
        public UpdateDeliveryOptionValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OptionId).NotEmpty();
            RuleFor(x => x.ExpectedVersion).GreaterThan(0);
            RuleFor(x => x.Input).NotNull().SetValidator(new DeliveryOptionInputValidator());
        }
    }
    public sealed class SetDeliveryOptionStatusValidator : AbstractValidator<SetDeliveryOptionStatusCommand>
    {
        public SetDeliveryOptionStatusValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OptionId).NotEmpty();
            RuleFor(x => x.ExpectedVersion).GreaterThan(0);
        }
    }
    public sealed class SaveFulfillmentValidator : AbstractValidator<SaveFulfillmentCommand>
    {
        public SaveFulfillmentValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.FulfillmentNotes).MaximumLength(500);
        }
    }
    public sealed class SaveProductDeliverySelectionValidator : AbstractValidator<SaveProductDeliverySelectionCommand>
    {
        public SaveProductDeliverySelectionValidator()
        {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.ExpectedVersion).GreaterThanOrEqualTo(0);
            RuleFor(x => x.DeliveryOptionIds).Must(ids =>
                ids is not null && ids.Count <= 50 &&
                ids.All(id => id != Guid.Empty) &&
                ids.Distinct().Count() == ids.Count)
                .WithMessage("Provide at most 50 distinct delivery option IDs; use [] to clear.");
        }
    }
}
