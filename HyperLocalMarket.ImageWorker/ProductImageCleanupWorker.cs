using HyperLocalMarket.Application.Images.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.ImageWorker
{
    public sealed class ProductImageCleanupWorker(
        IServiceScopeFactory scopes,
        ILogger<ProductImageCleanupWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = scopes.CreateScope();
                    await scope.ServiceProvider
                        .GetRequiredService<IProductImageCleanup>()
                        .CleanupAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                    break;
                }
                catch (Exception error) {
                    logger.LogError(
                        error, "Could not clean up expired product uploads.");
                }
            }
        }
    }

}
