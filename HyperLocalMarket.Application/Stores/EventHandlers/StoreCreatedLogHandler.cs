using HyperLocalMarket.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.EventHandlers
{
    public sealed class StoreCreatedLogHandler : INotificationHandler<StoreCreatedDomainEvent>
    {
        private readonly ILogger<StoreCreatedLogHandler> _logger;
        public StoreCreatedLogHandler(ILogger<StoreCreatedLogHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(StoreCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Store created. StoreId={StoreId}, Name={Name}, OccurredOnUtc={OccurredOnUtc}",
                notification.StoreId,
                notification.Name,
                notification.OccurredOnUtc
                );

            return Task.CompletedTask;

        }
    }
}
