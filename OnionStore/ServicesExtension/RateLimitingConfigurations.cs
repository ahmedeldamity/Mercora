using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace API.ServicesExtension
{
    public static class RateLimitingConfigurations
    {
        public static IServiceCollection AddRateLimitingConfigurations(this IServiceCollection services)
        {
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.AddFixedWindowLimiter("FixedWindowPolicy", options =>
                {
                    options.Window = TimeSpan.FromSeconds(5);
                    options.PermitLimit = 5; // 5 requests are allowed
                    options.QueueLimit = 10; // 10 requests can be queued if the limit is reached
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // oldest requests are processed first
                }).RejectionStatusCode = 429;

                rateLimiterOptions.AddSlidingWindowLimiter("SlidingWindowPolicy", options =>
                {
                    options.Window = TimeSpan.FromSeconds(5); // every 5 seconds
                    options.PermitLimit = 5; // 5 requests are allowed
                    options.QueueLimit = 10; // 10 requests can be queued if the limit is reached
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // oldest requests are processed first
                    options.SegmentsPerWindow = 5; // 5 segments in the window
                }).RejectionStatusCode = 429;

                rateLimiterOptions.AddConcurrencyLimiter("ConcurrencyPolicy", options =>
                {
                    options.PermitLimit = 1; // 5 requests are allowed for each user
                    options.QueueLimit = 2; // 10 requests can be queued if the limit is reached
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // oldest requests are processed first
                }).RejectionStatusCode = 429;

            });

            return services;
        }
    }
}
