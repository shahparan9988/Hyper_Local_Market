using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperLocalMarket.Application.Common.Behaviors;

namespace HyperLocalMarket.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            var applicationAssembly =
                typeof(DependencyInjection).Assembly;

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(
                    applicationAssembly);

                configuration.AddOpenBehavior(
                    typeof(ValidationBehavior<,>));
            });

            //Finds all public FluentValidation validators
            // inside the Application assembly.
            services.AddValidatorsFromAssembly(
                applicationAssembly);

            // Makes TimeProvider available through dependency injection.
            services.AddSingleton<TimeProvider>(
                TimeProvider.System);

            return services;
        }
    }
}
