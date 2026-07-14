using System.Net;
using System.Text.Json;
using ProductApi.Domain.Exceptions;

namespace ProductApi.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        switch (exception)
        {
            case NotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                var notFoundResponse = new { error = "Resource not found.", detail = exception.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(notFoundResponse, options));
                break;

            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var validationResponse = new
                {
                    error = "Validation failed.",
                    detail = exception.Message,
                    errors = validationEx.Errors
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(validationResponse, options));
                break;

            case ConflictException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                var conflictResponse = new { error = "Conflict.", detail = exception.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(conflictResponse, options));
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorResponse = new { error = "An error occurred while processing your request.", detail = exception.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
                break;
        }
    }
}
