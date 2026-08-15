using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Domain.Authorization;
using HyperLocalMarket.Domain.Categories;
using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Domain.Users;
using HyperLocalMarket.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CategoryProposal> CategoryProposals => Set<CategoryProposal>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<PlatformRole> PlatformRoles => Set<PlatformRole>();
        public DbSet<PlatformPermission> PlatformPermissions => Set<PlatformPermission>();
        public DbSet<PlatformRolePermission> PlatformRolePermissions => Set<PlatformRolePermission>();
        public DbSet<UserPlatformRole> UserPlatformRoles => Set<UserPlatformRole>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddDomainEventsToOutbox();

            foreach (var entry in ChangeTracker.Entries())
            {
                System.Diagnostics.Debug.WriteLine(
                    $"{entry.Entity.GetType().Name} - {entry.State}");
            }

            return base.SaveChangesAsync(cancellationToken);
        }


        private void AddDomainEventsToOutbox()
        {
            var aggregateRoots = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            var domainEvents = aggregateRoots
                .SelectMany(x => x.DomainEvents)
                .ToList();

            var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().FullName!,
                Content = JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType()),
                OccurredAtUtc = domainEvent.OccurredAtUtc,
                RetryCount = 0
            }).ToList();

            OutboxMessages.AddRange(outboxMessages);

            foreach (var aggregateRoot in aggregateRoots)
            {
                aggregateRoot.ClearDomainEvents();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    
    }
}
