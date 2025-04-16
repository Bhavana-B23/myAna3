using Newtonsoft.Json;
using System.Net;

namespace DataVizNavigator1.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Ajax request, return JSON error
                return context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    error = "An error occurred while processing your request.",
                    details = exception.Message
                }));
            }
            else
            {
                // Redirect to error page
                context.Response.Redirect("/Home/Error");
                return Task.CompletedTask;
            }
        }
    }

    // Extension method for using the middleware
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
