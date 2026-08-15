using HyperLocalMarket.Api.Authentication;
using HyperLocalMarket.Api.Authorization;
using HyperLocalMarket.Api.Middlewares;
using HyperLocalMarket.Application;
using HyperLocalMarket.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



//// MediatR: scan Application + Domain assemblies (for handlers + notifications)
//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssemblies(
//        typeof(HyperLocalMarket.Application.Stores.Commands.CreateStore.CreateStoreCommand).Assembly,
//        typeof(HyperLocalMarket.Domain.common.IDomainEvent).Assembly
//    );
//});


// MediatR, handlers, validators and pipeline behaviours
builder.Services.AddApplication();

// Infrastructure DI
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = SessionAuthenticationDefaults.SchemeName;
        options.DefaultChallengeScheme = SessionAuthenticationDefaults.SchemeName;
    })
    .AddScheme<
        AuthenticationSchemeOptions, 
        SessionAuthenticationHandler>(
            SessionAuthenticationDefaults.SchemeName,
            _ => { });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("ReactApp");

app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseMiddleware<SessionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
