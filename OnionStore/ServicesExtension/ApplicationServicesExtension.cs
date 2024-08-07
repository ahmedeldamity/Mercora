using API.EmailSetting;
using Core.Interfaces.EmailSetting;

namespace API.ServicesExtension
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register EmailSettings
            services.AddTransient(typeof(IEmailSettings), typeof(EmailSettings));

            return services;
        }
    }
}