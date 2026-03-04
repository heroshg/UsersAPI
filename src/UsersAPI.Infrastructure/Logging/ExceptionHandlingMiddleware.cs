using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using UsersAPI.Domain.Common;

namespace UsersAPI.Infrastructure.Logging
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            BaseLogger<ExceptionHandlingMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                logger.LogWarning(ex.ToString());
                await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message!);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                await WriteResponse(context, HttpStatusCode.InternalServerError, $"Unexpected error: {ex.Message}");
            }
        }

        private static async Task WriteResponse(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new
            {
                error = message,
                correlationId = context.Response.Headers["x-correlation-id"]
            });

            await context.Response.WriteAsync(response);
        }
    }
}
