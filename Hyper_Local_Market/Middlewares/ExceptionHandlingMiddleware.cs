using HyperLocalMarket.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HyperLocalMarket.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            var errors = exception.Errors.ToDictionary(
                error => error.Key,
                error => error.Value);

            var problemDetails =
                new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (UnauthorizedException exception)
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                error = exception.Message,
                message = exception.Message
            });
        }
        catch (ForbiddenException exception)
        {
            context.Response.StatusCode =
                StatusCodes.Status403Forbidden;

            await context.Response.WriteAsJsonAsync(new
            {
                error = exception.Message,
                message = exception.Message
            });
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(new
            {
                error = exception.Message,
                message = exception.Message
            });
        }
        catch (ConflictException exception)
        {
            context.Response.StatusCode =
                StatusCodes.Status409Conflict;

            await context.Response.WriteAsJsonAsync(new
            {
                error = exception.Message,
                message = exception.Message
            });
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogDebug(
                "Request was cancelled by the client: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }
        catch (HyperLocalMarket.Shared.Exceptions.DomainException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new {
                error = exception.Message,
                message = exception.Message
            });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "These details changed elsewhere. Reload before saving again.",
                message = "These details changed elsewhere. Reload before saving again."
            });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException exception)
            when (exception.InnerException is Npgsql.PostgresException { SqlState: "23505" or "23503" or "23514" })
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "A value already exists or a referenced item changed. Check the product, SKU and category selections." });
        }
        catch (Exception exception)
            when (exception is Npgsql.PostgresException { SqlState: "40001" or "40P01" } ||
            exception.InnerException is Npgsql.PostgresException { SqlState: "40001" or "40P01" })
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(
                new { message = "This information changed during the save. Reload before trying again." });
        }

        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal server error",
                message = "Internal server error"
            });
        }
    }
}