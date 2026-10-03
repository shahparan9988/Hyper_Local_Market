using FluentValidation;
using HyperLocalMarket.Application.Common.PhoneNumbers;
using HyperLocalMarket.Application.Common.TimeZones;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateContact;

public sealed class UpdateContactCommandValidator
    : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(command => command.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(
                "Phone number is required.")
            .MaximumLength(30)
            .WithMessage(
                "Phone number cannot exceed 30 characters.")
            .Must(phoneNumber =>
                SupportedPhoneNumber.TryNormalize(
                    phoneNumber,
                    out _))
            .WithMessage(
                "Enter a valid Bangladesh or Australian phone number.")
            .OverridePropertyName("phoneNumber");

        When(
            command =>
                !string.IsNullOrWhiteSpace(
                    command.Email),
            () =>
            {
                RuleFor(command => command.Email)
                    .MaximumLength(320)
                    .WithMessage(
                        "Email cannot exceed 320 characters.")
                    .EmailAddress()
                    .WithMessage(
                        "Enter a valid email address.")
                    .OverridePropertyName("email");
            });

        RuleFor(command => command.TimeZoneId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(
                "Timezone is required.")
            .Must(
                SupportedStoreTimeZones.IsSupported)
            .WithMessage(
                "Select a supported timezone.")
            .OverridePropertyName("timeZoneId");
    }
}