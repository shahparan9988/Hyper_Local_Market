using Amazon.SQS;
using Amazon.SQS.Model;
using HyperLocalMarket.Application.Images.Commands.MarkProductImageFailed;
using HyperLocalMarket.Application.Images.Commands.MarkStoreBrandingImageFailed;
using HyperLocalMarket.Application.Images.Commands.ProcessProductImage;
using HyperLocalMarket.Application.Images.Commands.ProcessStoreBrandingImage;
using HyperLocalMarket.Infrastructure.External.Images;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace HyperLocalMarket.ImageWorker
{
    public sealed class Worker(
        IAmazonSQS sqs,
        IServiceScopeFactory scopes,
        IOptions<ImageQueueOptions> queue,
        IOptions<ImageStorageOptions> storage,
        ILogger<Worker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                    {
                        QueueUrl = queue.Value.QueueUrl,
                        MaxNumberOfMessages = 1,
                        WaitTimeSeconds = 20,
                        VisibilityTimeout = 120,
                        MessageSystemAttributeNames = ["ApproximateReceiveCount"]
                    }, stoppingToken);
                    foreach (var message in result.Messages ?? [])
                        await ProcessMessage(message, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                    break;
                }
                catch (Exception error)
                {
                    logger.LogError(error, "Image queue polling failed.");
                    try { 
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                    catch 
                    (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                        break;
                    }
                }
            }
        }
        private async Task ProcessMessage(Message message, CancellationToken stoppingToken)
        {
            var count = message.Attributes is not null 
                && message.Attributes.TryGetValue("ApproximateReceiveCount", out var value)
                && int.TryParse(value, out var parsed) ? parsed : 1;

            using var heartbeatToken = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            var heartbeat = RenewVisibility(message, heartbeatToken.Token);
            try
            {
                using var body = JsonDocument.Parse(message.Body);
                if (body.RootElement.TryGetProperty("Event", out var test)
                    && test.GetString() == "s3:TestEvent")
                {
                    await Delete(message, stoppingToken); return;
                }
                var envelope = JsonSerializer.Deserialize<S3EventEnvelope>(message.Body);
                if (envelope?.Records is null)
                    throw new InvalidOperationException("Expected a direct S3 notification.");
                var keys = envelope.Records
                    .Where(x => x.EventName.StartsWith("ObjectCreated:", StringComparison.Ordinal)
                        && x.S3.Bucket.Name == storage.Value.BucketName)
                    .Select(x => WebUtility.UrlDecode(x.S3.Object.Key))
                    .Where(x => IsProduct(x) 
                        || x.StartsWith("incoming/stores/", StringComparison.Ordinal)
                        && x.Contains("/branding/", StringComparison.Ordinal))
                    .Distinct(StringComparer.Ordinal).ToList();
                var failed = false;
                foreach (var key in keys)
                {
                    try
                    {
                        using var scope = scopes.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        deadline.CancelAfter(TimeSpan.FromMinutes(2));
                        if (IsProduct(key))
                            await mediator.Send(new ProcessProductImageCommand(key), deadline.Token);
                        else
                            await mediator.Send(new ProcessStoreBrandingImageCommand(key), deadline.Token);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                        throw;
                    }
                    catch (Exception error)
                    {
                        failed = true;
                        logger.LogError(error, "Image {Key} failed on attempt {Count}.", key, count);
                        if (count >= queue.Value.MaximumReceiveCount)
                        {
                            using var scope = scopes.CreateScope();
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            try
                            {
                                if (IsProduct(key))
                                    await mediator.Send(
                                        new MarkProductImageFailedCommand(key), stoppingToken);
                                else
                                    await mediator.Send(
                                        new MarkStoreBrandingImageFailedCommand(key, "Image processing failed after multiple attempts."), stoppingToken);
                            }
                            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                                throw;
                            }
                            catch (Exception markError) {
                                logger.LogError(markError, "Could not record failure for {Key}.", key);
                            }
                        }
                    }
                }
                // Process every record even if another record failed. Successful records are
                // terminal/idempotent on retry. A failed message remains for retry/DLQ redrive.
                if (!failed)
                    await Delete(message, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception error) {
                logger.LogError(error, "Image message {Id} could not be handled.", message.MessageId);
            }
            finally
            {
                await heartbeatToken.CancelAsync();
                try {
                    await heartbeat;
                }
                catch (OperationCanceledException) when (heartbeatToken.IsCancellationRequested) { }
                catch (Exception error) {
                    logger.LogWarning(
                        error, "Could not renew visibility for message {Id}.", message.MessageId);
                }
            }
        }
        private async Task RenewVisibility(Message message, CancellationToken ct)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(40));
            while (await timer.WaitForNextTickAsync(ct))
            {
                await sqs.ChangeMessageVisibilityAsync(new ChangeMessageVisibilityRequest
                { QueueUrl = queue.Value.QueueUrl, ReceiptHandle = message.ReceiptHandle, VisibilityTimeout = 120 }, ct);
            }
        }
        private Task Delete(Message message, CancellationToken ct) =>
            sqs.DeleteMessageAsync(queue.Value.QueueUrl, message.ReceiptHandle, ct);
        private static bool IsProduct(string key) =>
            key.StartsWith("incoming/products/", StringComparison.Ordinal);
    }

}
