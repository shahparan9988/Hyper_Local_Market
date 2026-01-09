using HyperLocalMarket.Application.Common.Interfaces;
using HyperLocalMarket.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.EventHandlers
{
    public sealed class StoreCreatedEmailHandler : INotificationHandler<StoreCreatedDomainEvent>
    {
        private readonly IEmailService _emailService;
        public StoreCreatedEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(StoreCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            const string to = "Admin@hyperLocalMarket.com";
            var subject = $"New Store Created: {notification.Name}";
            var body = $"A store was created.\n\nStoreId: {notification.StoreId}\nName: {notification.Name}\nTime: {notification.OccurredOnUtc:u}";

            await _emailService.SendAsync(to, subject, body, cancellationToken);
        }
    }
}
