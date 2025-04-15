using System.Text.Json;
using FlagExplorerApp.Api.Models;

namespace FlagExplorerApp.Api.Middlewares
{

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
                // Pass control to the next middleware in the pipeline
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                // Handle client-side cancellations
                _logger.LogWarning("The operation was canceled by the client.");
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "The operation was canceled by the client.");
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related exceptions
                _logger.LogWarning(ex, "A client-side error occurred.");
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message, ex.StackTrace);
            }
            catch (Exception ex)
            {
                // Handle unexpected server-side exceptions
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.", ex.Message);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, int statusCode, string message, string? details = null)
        {
            // Set the response status code and content type
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            // Create a standard error response
            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Details = details // Include detailed information only if provided
            };

            // Serialize the response to JSON and write it to the response body
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

}
