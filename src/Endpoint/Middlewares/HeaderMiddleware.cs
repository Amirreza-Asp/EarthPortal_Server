namespace Endpoint.Middlewares
{
    public static class HeaderHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomHeaderHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HeaderHandlerMiddleware>();
        }
    }

    public class HeaderHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HeaderHandlerMiddleware> _logger;

        public HeaderHandlerMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<HeaderHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.ToLower().Contains("image"))
            {
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-AspNet-Version");
                context.Response.Headers.Remove("X-AspNetMvc-Version");
                context.Response.Headers.Remove("X-Frame-Options");
                context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';base-uri 'self';font-src 'self';form-action 'self';frame-ancestors 'self';img-src 'self' data:;object-src 'none';script-src 'self';script-src-attr 'none';style-src 'self' https: 'unsafe-inline';upgrade-insecure-requests");
                context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
                context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
                context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
                context.Response.Headers.Append("Referrer-Policy", "no-referrer");
                context.Response.Headers.Append("Strict-Transport-Security", "max-age=63072000; includeSubDomains perload");
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("Origin-Agent-Cluster", "?1");
                context.Response.Headers.Append("X-DNS-Prefetch-Control", "off");
                context.Response.Headers.Append("X-Download-Options", "noopen");
                context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            }

            await _next(context);
        }
    }

}
