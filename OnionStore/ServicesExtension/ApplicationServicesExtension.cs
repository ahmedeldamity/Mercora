using API.EmailSetting;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using DotNetCore_ECommerce.Helpers;
using Repository;
using Service;

namespace API.ServicesExtension
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Unit Of Work
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            // Register EmailSettings
            services.AddTransient(typeof(IEmailSettings), typeof(EmailSettings));

            // Register AuthService
            services.AddScoped(typeof(IAuthService), typeof(AuthService));

            // Register Product Service
            services.AddScoped(typeof(IProductService), typeof(ProductService));

            // Register Category Service
            services.AddScoped(typeof(ICategoryService), typeof(CategoryService));

            // --- Two Ways To Register AutoMapper
            // - First (harder)
            //builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
            // - Second (easier)
            services.AddAutoMapper(typeof(MappingProfiles));

            return services;
        }
    }
}