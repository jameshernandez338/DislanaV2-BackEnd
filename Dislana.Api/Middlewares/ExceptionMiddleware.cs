using Dislana.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace Dislana.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain validation error.");

                await WriteProblemAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred.");

                await WriteProblemAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Error interno del servidor.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                await WriteProblemAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Error interno del servidor.");
            }
        }

        private static async Task WriteProblemAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            var traceId = context.TraceIdentifier;

            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = message,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(problem));
        }
    }
}