using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Domain.Common;
using Microsoft.Extensions.Logging;

namespace ContactBookAPI.Application.Common.Behaviours;

public class DomainExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;

    public DomainExceptionHandlingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (BaseDomainException exception)
        {
            _logger.LogWarning(exception, "Domain Exception for Request {Name}: {Message}",
                typeof(TRequest).Name, exception.Message);

            // Handle Result<T>
            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                // Get the generic type argument
                var valueType = typeof(TResponse).GetGenericArguments()[0];

                // Create the appropriate Result<T>.Failure
                var failureMethod = typeof(Result<>)
                    .MakeGenericType(valueType)
                    .GetMethod("Failure", new[] { typeof(IEnumerable<string>) });

                if (failureMethod != null)
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
                    return (TResponse)failureMethod.Invoke(null, new object[] { new[] { exception.Error } });
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
            }
            // Handle non-generic Result
            else if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(new[] { exception.Error });
            }

            // If not a Result type, rethrow the exception
            throw;
        }
    }
}
