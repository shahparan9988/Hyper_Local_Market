using Amazon.S3;
using Amazon.SQS;
using HyperLocalMarket.Application.Auth.Repositories;
using HyperLocalMarket.Application.Auth.Services;
using HyperLocalMarket.Application.Authorization.Repositories;
using HyperLocalMarket.Application.Authorization.Services;
using HyperLocalMarket.Application.Common.Interfaces;
using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Images.Services;
using HyperLocalMarket.Application.Inventory.Queries;
using HyperLocalMarket.Application.Products.Queries;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Stores.Queries.GetMyStores;
using HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Application.Stores.Services;
using HyperLocalMarket.Infrastructure.External.Emaill;
using HyperLocalMarket.Infrastructure.External.Images;
using HyperLocalMarket.Infrastructure.Persistence.Outbox;
using HyperLocalMarket.Infrastructure.Persistence.Queries.Inventory;
using HyperLocalMarket.Infrastructure.Persistence.Queries.Products;
using HyperLocalMarket.Infrastructure.Persistence.Queries.Stores;
using HyperLocalMarket.Infrastructure.Persistence.Repositories.Auth;
using HyperLocalMarket.Infrastructure.Persistence.Repositories.Authorization;
using HyperLocalMarket.Infrastructure.Persistence.Repositories.Images;
using HyperLocalMarket.Infrastructure.Persistence.Repositories.Products;
using HyperLocalMarket.Infrastructure.Persistence.Repositories.Stores;
using HyperLocalMarket.Infrastructure.Security.Authentication;
using HyperLocalMarket.Infrastructure.Security.Authorization;
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
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            bool addOutboxProcessor = true)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                "DefaultConnection was not configured.");

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
            

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasher, AppPasswordHasher>();
            services.AddSingleton<IStoreSlugService, StoreSlugService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductCatalogRepository, ProductCatalogRepository>();
            services.AddScoped<IProductCatalogReader, ProductCatalogReader>();
            services.AddScoped<IInventoryReader, InventoryReader>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductPhotoEncoder, ProductPhotoEncoder>();
            services.AddScoped<IProductImageCleanup, ProductImageCleanup>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IPlatformAuthorizationRepository, PlatformAuthorizationRepository>();
            services.AddScoped<IPlatformPermissionChecker, PlatformPermissionChecker>();
            services.AddScoped<IStoreSetupStatusReader, StoreSetupStatusReader>();
            services.AddScoped<IMyStoresReader, MyStoresReader>();
            services.AddScoped<IDeliveryOptionRepository, DeliveryOptionRepository>();
            //services.AddScoped<ICategoryRepository, CategoryRepository>();


            var awsOptions = configuration.GetAWSOptions();

            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonSQS>();

            services
                .AddOptions<ImageStorageOptions>()
                .Bind(configuration.GetSection(ImageStorageOptions.SectionName))
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.BucketName),
                    "ImageStorage:BucketName is required.")
                .Validate(
                    options => !string.IsNullOrWhiteSpace(
                        options.CloudFrontBaseUrl),
                    "ImageStorage:CloudFrontBaseUrl is required.")
                .ValidateOnStart();

            services.AddScoped<
                IStoreBrandingImageRepository,
                StoreBrandingImageRepository>();

            services.AddScoped<IImageStorage, S3ImageStorage>();

            services.AddScoped<
                IStoreBrandingImageProcessor,
                StoreBrandingImageProcessor>();

            services.AddSingleton<
                IImageObjectKeyFactory,
                ImageObjectKeyFactory>();



            if (addOutboxProcessor)
            {
                services.AddHostedService<OutboxProcessorBackgroundService>();
            }

            return services;
        }
    }
}
