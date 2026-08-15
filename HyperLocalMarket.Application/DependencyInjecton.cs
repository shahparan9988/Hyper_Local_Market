using FluentValidation;
using HyperLocalMarket.Application.Common.Abstractions;
using HyperLocalMarket.Application.Common.Behaviors;
using HyperLocalMarket.Application.Common.Slugs;
using HyperLocalMarket.Application.Products.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            services.AddScoped<
                IProductSlugService,
                ProductSlugService>();

            services.AddScoped<
                ISlugGenerator,
                SlugGenerator>();

            // Makes TimeProvider available through dependency injection.
            services.AddSingleton<TimeProvider>(
                TimeProvider.System);

            return services;
        }
    }
}
