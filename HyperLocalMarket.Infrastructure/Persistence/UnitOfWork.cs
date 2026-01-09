using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Domain.common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IMediator _mediator;
        public UnitOfWork(AppDbContext appDbContext, IMediator mediator)
        {
            _dbContext = appDbContext;
            _mediator = mediator;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // Collect domain events BEFORE saving
            var domainEvents = await CollectDomainEventsAsync(ct);

            var result = await _dbContext.SaveChangesAsync(ct);

            // Publish domain events AFTER successful save
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, ct);
            }

            return result;
        }

        private async Task<List<IDomainEvent>> CollectDomainEventsAsync(CancellationToken ct)
        {
            // Make sure change tracker is up to date
            _dbContext.ChangeTracker.DetectChanges();

            var entities = _dbContext.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var events = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            foreach (var entity in entities)
            {
                entity.ClearDomainEvents();
            }

            return events;
        }
    }
}
