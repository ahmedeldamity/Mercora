namespace BlazorEcommerce.Server.ServicesExtension;
public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Unit Of Work
        services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

        // Register EmailSettings
        services.AddTransient(typeof(IEmailSettingService), typeof(EmailSettingService));

        // Register Product Service
        services.AddScoped(typeof(IProductService), typeof(ProductService));

        // Register Category Service
        services.AddScoped(typeof(ICategoryService), typeof(CategoryService));

        // Register Brand Service
        services.AddScoped(typeof(IBrandService), typeof(BrandService));

        // Register Cart Repository
        services.AddScoped(typeof(IBasketRepository), typeof(CartRepository));

        // Register Order Service
        services.AddScoped(typeof(IOrderService), typeof(OrderService));

        // Register Payment Service
        services.AddScoped(typeof(IPaymentService), typeof(PaymentService));

        // Register Delivery Method Service
        services.AddScoped(typeof(IDeliveryMethodService), typeof(DeliveryMethodService));

        // Register Cart Service
        services.AddScoped(typeof(ICartService), typeof(CartService));

        // Register Account Service 
        services.AddScoped(typeof(IAccountService), typeof(AccountService));

        // Register Caching service
        services.AddSingleton(typeof(IResponseCacheService), typeof(ResponseCacheService));

        // --- Two Ways To Register AutoMapper
        // - First (harder)
        //builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
        // - Second (easier)
        services.AddAutoMapper(typeof(MappingProfiles));

        return services;
    }
}