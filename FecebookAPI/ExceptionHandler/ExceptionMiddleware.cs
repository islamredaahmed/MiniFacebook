using FecebookAPI.Log;
using FecebookAPI.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace FecebookAPI.ExceptionHandler
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (CustomValidationException vEx)
            {
                _logger.LogError($"{vEx.Message}, \r\n Action Name: {vEx.TargetSite?.Name},\r\n Class Name: {vEx.TargetSite?.DeclaringType}");
                await HandleExceptionAsync(httpContext, vEx);
            }catch (CustomAuthorizationException aEx)
            {
                _logger.LogError($"{aEx.Message}, \r\n Action Name: {aEx.TargetSite?.Name},\r\n Class Name: {aEx.TargetSite?.DeclaringType}");
                await HandleExceptionAsync(httpContext, aEx);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message}, \r\n Action Name: {ex.TargetSite?.Name},\r\n Class Name: {ex.TargetSite?.DeclaringType}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var message = exception switch
            {
                CustomValidationException => $"{exception.Message}, \r\n Action Name: {exception.TargetSite?.Name},\r\n Class Name: {exception.TargetSite?.DeclaringType}",
                CustomAuthorizationException => $"{exception.Message}, \r\n Action Name: {exception.TargetSite?.Name},\r\n Class Name: {exception.TargetSite?.DeclaringType}",
                _ => $"{exception.Message},\r\n Action Name: {exception.TargetSite?.Name},\r\n Class Name: {exception.TargetSite?.DeclaringType}"
            };
            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
