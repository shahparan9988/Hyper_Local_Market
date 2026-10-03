using HyperLocalMarket.ImageWorker;
using HyperLocalMarket.Infrastructure.Persistence;
using HyperLocalMarket.Application;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration,
    addOutboxProcessor: false);

builder.Services
    .AddOptions<ImageQueueOptions>()
    .Bind(
        builder.Configuration.GetSection(
            ImageQueueOptions.SectionName))
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.QueueUrl),
        "ImageQueue:QueueUrl is required.")
    .ValidateOnStart();

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<ProductImageCleanupWorker>();

var host = builder.Build();

await host.RunAsync();
