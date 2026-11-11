using System.Net;
using System.Text.Json;

namespace SchoolManagement.Web.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred during request execution: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // If AJAX / JSON request
        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
            (context.Request.ContentType?.Contains("application/json") ?? false) ||
            context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                succeeded = false,
                message = "An unexpected error occurred while processing your request. Please try again or contact system administration.",
                details = exception.Message
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        else
        {
            // Redirect to standard error page or show user-friendly error
            context.Response.Redirect("/Home/Error");
        }
    }
}

