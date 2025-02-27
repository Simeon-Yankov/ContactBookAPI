using Serilog;

namespace ContactBookAPI.Web.Infrastructure;

public class PersonRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public PersonRequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;

        if (request.Path.StartsWithSegments("/api/people", out var remainingPath))
        {
            var hasId = request.Query.TryGetValue("Id", out var personId);
            if (hasId)
            {
                Log.Information(
                    "Request received: {Method} {Path}, Person ID: {PersonId}",
                    request.Method,
                    request.Path,
                    personId);
            }
        }

        await _next(context);
    }
}
