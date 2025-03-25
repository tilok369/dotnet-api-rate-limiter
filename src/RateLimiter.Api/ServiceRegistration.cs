using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace RateLimiter.Api;

public static class ServiceRegistration
{
    public static IServiceCollection AddFixedWindowRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed-window", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromSeconds(10);
                limiterOptions.QueueLimit = 0;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
            
            //options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            options.OnRejected = async (context, cancellationToken) =>
            {
                // Custom rejection handling logic
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers["Retry-After"] = "10s";

                await context.HttpContext.Response.WriteAsync("Too many requests! Please try again later.", cancellationToken);
            };
        });
        
        return services;
    }
    
    public static IServiceCollection AddSlidingWindowRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddSlidingWindowLimiter("sliding-window", rateLimitOptions =>
            {
                rateLimitOptions.PermitLimit = 5;
                rateLimitOptions.Window = TimeSpan.FromSeconds(10);
                rateLimitOptions.SegmentsPerWindow = 5;
                rateLimitOptions.QueueLimit = 0;
                rateLimitOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
            
            options.OnRejected = async (context, cancellationToken) =>
            {
                // Custom rejection handling logic
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers["Retry-After"] = "10s";

                await context.HttpContext.Response.WriteAsync("Too many requests! Please try again later.", cancellationToken);
            };
        });
        
        return services;
    }
    
    public static IServiceCollection AddTokenBucketRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter("token-bucket", rateLimitOptions =>
            {
                rateLimitOptions.TokenLimit = 5;
                rateLimitOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                rateLimitOptions.TokensPerPeriod = 3;
                rateLimitOptions.QueueLimit = 0;
                rateLimitOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                rateLimitOptions.AutoReplenishment = true;
            });
            
            options.OnRejected = async (context, cancellationToken) =>
            {
                // Custom rejection handling logic
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers["Retry-After"] = "10s";

                await context.HttpContext.Response.WriteAsync("Too many requests! Please try again later.", cancellationToken);
            };
        });
        
        return services;
    }
}