using System.Net;
using System.Net.Http;
using System.Text.Json;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using PaymentGateway.CrossCutting.Models;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.CrossCutting.Middleware
{
    public class InterceptorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<InterceptorHandlingMiddleware> _logger;

        public InterceptorHandlingMiddleware(RequestDelegate next, ILogger<InterceptorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httContext)
        {
            try
            {
                await _next(httContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            if (exception is BusinessException)
            {
                _logger.LogError(exception, "Business validation failed: {Message}", exception.Message);
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (exception is ValidationException)
            {
                _logger.LogWarning(exception, "Validation failed: {Message}", exception.Message);
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (exception is NotFoundException)
            {
                _logger.LogWarning(exception, "Resource not found: {Message}", exception.Message);
                statusCode = HttpStatusCode.NotFound;
            }
            else if (exception is HttpRequestException)
            {
                _logger.LogWarning(exception, "HTTP request failed: {Message}", exception.Message);
                statusCode = HttpStatusCode.ServiceUnavailable;
            }
            else if (exception is OperationCanceledException)
            {
                _logger.LogWarning(exception, "Operation was cancelled: {Message}", exception.Message);
                statusCode = HttpStatusCode.BadRequest;
            }
            else
            {
                _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
            }

            var response = new ErrorResponseModel
            {
                StatusCode = (int)statusCode,
                Message = exception.Message,
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(result);
        }
    }
}
