using HyperLocalMarket.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/hyperlocalmarket-.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR: scan Application + Domain assemblies (for handlers + notifications)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(HyperLocalMarket.Application.Stores.Commands.CreateStore.CreateStoreCommand).Assembly,
        typeof(HyperLocalMarket.Domain.common.IDomainEvent).Assembly
    );
});

// Infrastructure DI
builder.Services.AddInfrastructure(builder.Configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
