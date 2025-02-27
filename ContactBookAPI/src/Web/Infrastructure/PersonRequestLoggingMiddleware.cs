namespace ContactBookAPI.Web.Infrastructure;

public class PersonRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PersonRequestLoggingMiddleware> _logger;

    public PersonRequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<PersonRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;

        if (request.Path.StartsWithSegments("/api/people", out var remainingPath))
        {
            var hasId = request.Query.TryGetValue("Id", out var personId);
            if (hasId)
            {
                _logger.LogInformation(
                    "Request received: {Method} {Path}, Person ID: {PersonId}",
                    request.Method,
                    request.Path,
                    personId);
            }
        }

        await _next(context);
    }
}
