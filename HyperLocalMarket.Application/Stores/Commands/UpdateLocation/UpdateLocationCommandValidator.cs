using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateLocation
{

    public sealed class UpdateLocationCommandValidator
    : AbstractValidator<UpdateLocationCommand>
    {
        public UpdateLocationCommandValidator()
        {
            RuleFor(command => command.StoreId)
                .NotEmpty();

            RuleFor(command => command.UserId)
                .NotEmpty();

            RuleFor(command => command.CountryCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Country code is required.")
                .Matches("^[A-Za-z]{2}$")
                .WithMessage(
                    "Country code must be a two-letter ISO country code.")
                .OverridePropertyName("countryCode");

            RuleFor(command => command.Latitude)
                .NotNull()
                .WithMessage("Latitude is required.")
                .Must(latitude =>
                    !latitude.HasValue ||
                    latitude.Value is >= -90 and <= 90)
                .WithMessage("Latitude must be between -90 and 90.")
                .OverridePropertyName("coordinates.latitude");

            RuleFor(command => command.Longitude)
                .NotNull()
                .WithMessage("Longitude is required.")
                .Must(longitude =>
                    !longitude.HasValue ||
                    longitude.Value is >= -180 and <= 180)
                .WithMessage("Longitude must be between -180 and 180.")
                .OverridePropertyName("coordinates.longitude");

            RuleFor(command => command.AddressLine1)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Address line 1 is required.")
                .Must(value =>
                    value is not null &&
                    value.Trim().Length <= 300)
                .WithMessage(
                    "Address line 1 cannot exceed 300 characters.")
                .OverridePropertyName("address.addressLine1");

            ValidateOptionalText(
                command => command.AddressLine2,
                300,
                "address.addressLine2",
                "Address line 2");

            ValidateOptionalText(
                command => command.Locality,
                150,
                "address.locality",
                "Locality");

            ValidateOptionalText(
                command => command.Region,
                150,
                "address.region",
                "Region");

            ValidateOptionalText(
                command => command.Postcode,
                30,
                "address.postcode",
                "Postcode");

            ValidateOptionalText(
                command => command.Landmark,
                300,
                "address.landmark",
                "Landmark");

            ValidateOptionalText(
                command => command.Level1,
                200,
                "adminArea.level1",
                "Administrative level 1");

            ValidateOptionalText(
                command => command.Level2,
                200,
                "adminArea.level2",
                "Administrative level 2");

            ValidateOptionalText(
                command => command.Level3,
                200,
                "adminArea.level3",
                "Administrative level 3");

            ValidateOptionalText(
                command => command.Level4,
                200,
                "adminArea.level4",
                "Administrative level 4");

            RuleFor(command => command.Level1Id)
                .Must(id => !id.HasValue || id.Value > 0)
                .WithMessage(
                    "Administrative level 1 ID must be greater than zero.")
                .OverridePropertyName("adminArea.level1Id");

            RuleFor(command => command.Level2Id)
                .Must(id => !id.HasValue || id.Value > 0)
                .WithMessage(
                    "Administrative level 2 ID must be greater than zero.")
                .OverridePropertyName("adminArea.level2Id");

            RuleFor(command => command.Level3Id)
                .Must(id => !id.HasValue || id.Value > 0)
                .WithMessage(
                    "Administrative level 3 ID must be greater than zero.")
                .OverridePropertyName("adminArea.level3Id");
        }

        private void ValidateOptionalText(
            System.Linq.Expressions.Expression<
                Func<UpdateLocationCommand, string?>> property,
            int maximumLength,
            string propertyName,
            string displayName)
        {
            RuleFor(property)
                .Must(value =>
                    string.IsNullOrWhiteSpace(value) ||
                    value.Trim().Length <= maximumLength)
                .WithMessage(
                    $"{displayName} cannot exceed " +
                    $"{maximumLength} characters.")
                .OverridePropertyName(propertyName);
        }
    }
}
