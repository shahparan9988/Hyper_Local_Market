using HyperLocalMarket.Application.Common.Interfaces;
using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Stores.Services;
using HyperLocalMarket.Infrastructure.Persistence.Outbox;
using HyperLocalMarket.Infrastructure.Persistence.Repositories;
using HyperLocalMarket.Infrastructure.Security;
using HyperLocalMarket.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                "DefaultConnection was not configured.");

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
            
            services.AddScoped<IStoreRepository, StoreRepository>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasher, AppPasswordHasher>();
            services.AddSingleton<IStoreSlugService, StoreSlugService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            //services.AddScoped<ICategoryRepository, CategoryRepository>();

            /*
            * CreateProductCommandHandler depends on TimeProvider.
            */
            services.AddSingleton<TimeProvider>(
                TimeProvider.System);

            services.AddHostedService<OutboxProcessorBackgroundService>();

            return services;
        }
    }
}
