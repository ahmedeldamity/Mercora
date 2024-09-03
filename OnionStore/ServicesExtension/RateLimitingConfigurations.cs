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
                    options.PermitLimit = 5;
                    options.QueueLimit = 10;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = 429;
            });
                
            return services;
        }
    }
}
