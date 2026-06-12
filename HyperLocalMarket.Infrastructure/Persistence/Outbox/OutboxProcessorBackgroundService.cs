using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Outbox
{
    public sealed class OutboxProcessorBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessorBackgroundService> _logger;

        public OutboxProcessorBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessOutboxMessages(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task ProcessOutboxMessages(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var messages = await dbContext.OutboxMessages
                .Where(x => x.ProcessedAtUtc == null && x.RetryCount < 5)
                .OrderBy(x => x.OccurredAtUtc)
                .Take(20)
                .ToListAsync(ct);

            foreach (var message in messages)
            {
                try
                {
                    _logger.LogInformation(
                        "Processing outbox message {MessageId} of type {Type}",
                        message.Id,
                        message.Type);

                    message.ProcessedAtUtc = DateTime.UtcNow;
                    message.Error = null;
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;

                    _logger.LogError(
                        ex,
                        "Failed to process outbox message {MessageId}",
                        message.Id);
                }
            }

            await dbContext.SaveChangesAsync(ct);
        }
    }
}
