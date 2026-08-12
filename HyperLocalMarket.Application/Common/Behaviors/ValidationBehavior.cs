using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var validators = _validators.ToList();

            if (validators.Count == 0)
            {
                return await next(cancellationToken);
            }

            var validationContext =
                new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(validator =>
                    validator.ValidateAsync(
                        validationContext,
                        cancellationToken)));

            var validationFailures = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .ToList();

            if (validationFailures.Count != 0)
            {
                throw new ValidationException(
                    validationFailures);
            }

            return await next(cancellationToken);
        }
    }
}
