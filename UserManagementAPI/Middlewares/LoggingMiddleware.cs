
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MiddlewareDemo
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;

            await _next(context);

            var status = context.Response.StatusCode;
            Console.WriteLine($"{method} {path} -> {status}");
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSimpleLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggingMiddleware>();
        }
    }
}
