using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.CreateStoreBrandingUpload
{
    public sealed class CreateStoreBrandingUploadCommandValidator
    : AbstractValidator<CreateStoreBrandingUploadCommand>
    {
        public CreateStoreBrandingUploadCommandValidator()
        {
            RuleFor(command => command.StoreId).NotEmpty();
            RuleFor(command => command.UserId).NotEmpty();

            RuleFor(command => command.Kind)
                .IsInEnum();

            RuleFor(command => command.FileName)
                .NotEmpty()
                .MaximumLength(255)
                .OverridePropertyName("fileName");

            RuleFor(command => command.ContentType)
                .Must(StoreBrandingImageRules.AllowedContentTypes.Contains)
                .WithMessage("Only JPEG, PNG and WebP images are allowed.")
                .OverridePropertyName("contentType");

            RuleFor(command => command.FileSizeBytes)
                .GreaterThan(0)
                .LessThanOrEqualTo(StoreBrandingImageRules.MaximumUploadBytes)
                .WithMessage("The image cannot exceed 10 MB.")
                .OverridePropertyName("fileSizeBytes");
        }
    }
}
