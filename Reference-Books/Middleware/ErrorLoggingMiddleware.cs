namespace Reference_Books.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly ILogger logger;
        private readonly RequestDelegate next;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
            }
        }
    }
}
