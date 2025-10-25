using System.Net;
using System.Text.Json;

namespace Blogtify.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        HttpStatusCode statusCode;
        string errorCode;
        string message = exception.Message;

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorCode = "not_found";
                _logger.LogWarning(exception, "NotFoundException: {Message}", exception.Message);
                break;

            case BadRequestException:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "bad_request";
                _logger.LogWarning(exception, "BadRequestException: {Message}", exception.Message);
                break;

            case ConflictException:
                statusCode = HttpStatusCode.Conflict;
                errorCode = "conflict";
                _logger.LogWarning(exception, "ConflictException: {Message}", exception.Message);
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                errorCode = "internal_server_error";
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = errorCode,
            message,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
