using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace Uagrm.Sw1.iRecycle.Bff.Api.Middlewares;

internal sealed class CustomExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

    public CustomExceptionHandlerMiddleware(ILogger<CustomExceptionHandlerMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string requestBody = await HandleRequestLogging(context);

        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Body: {body}", requestBody);
            _logger.LogError(e, "{message}", e.Message);

            await HandleExceptionAsync(context, e);
        }
    }


    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var errors = GetErrors(exception) ?? new Dictionary<string, string[]>();
        var response = new ValidationProblemDetails(errors)
        {
            Title = "Server Error",
            Status = GetStatusCode(exception),
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = GetStatusCode(exception);

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }


    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            HttpRequestException httpRequest => (int?)httpRequest.StatusCode ?? StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

    private static IDictionary<string, string[]>? GetErrors(Exception exception)
    {
        if (exception is ValidationException validationException)
        {
            return (IDictionary<string, string[]>)validationException.Data;
        }

        return null;
    }


    private static async Task<string> HandleRequestLogging(HttpContext context)
    {
        context.Request.EnableBuffering();
        string requestBody;
        using (StreamReader reader = new(context.Request.Body, Encoding.UTF8, true, 1024, true))
        {
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        return requestBody;
    }

}
