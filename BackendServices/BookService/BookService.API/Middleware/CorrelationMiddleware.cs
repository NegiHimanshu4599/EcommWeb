namespace BookService.API.Middleware
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()?? Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            await _next(context);
        }
    }
}