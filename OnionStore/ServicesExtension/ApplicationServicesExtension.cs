using API.EmailSetting;
using Core.Interfaces.Services;
using Service;

namespace API.ServicesExtension
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register EmailSettings
            services.AddTransient(typeof(IEmailSettings), typeof(EmailSettings));

            // Register AuthService
            services.AddScoped(typeof(IAuthService), typeof(AuthService));

            return services;
        }
    }
}