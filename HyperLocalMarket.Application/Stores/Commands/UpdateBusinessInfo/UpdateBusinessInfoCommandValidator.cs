using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateBusinessInfo
{
    public sealed class UpdateBusinessInfoCommandValidator
        : AbstractValidator<UpdateBusinessInfoCommand>
    {
        public UpdateBusinessInfoCommandValidator()
        {
            RuleFor(command => command.StoreId)
                .NotEmpty();

            RuleFor(command => command.UserId)
                .NotEmpty();

            RuleFor(command => command.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Store name is required.")
                .Must(name => name.Trim().Length <= 200)
                .WithMessage("Store name cannot exceed 200 characters.")
                .OverridePropertyName("name");

            RuleFor(command => command.Description)
                .Must(description =>
                    string.IsNullOrWhiteSpace(description) ||
                    description.Trim().Length <= 2000)
                .WithMessage("Description cannot exceed 2000 characters.")
                .OverridePropertyName("description");
        }
    }
}
