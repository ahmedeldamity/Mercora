using Asp.Versioning;

namespace API.ServicesExtension;
public static class ApiVersioningConfigurations
{
    public static IServiceCollection AddApiVersioningConfigurations(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);   // This sets the default API version to 1.0
            options.AssumeDefaultVersionWhenUnspecified = true; // this option tells the application to assume the DefaultApiVersion (1.0 in this case) when the client doesn't specify an API version
            options.ReportApiVersions = true;					// the response headers will include information about the available API versions. This can be useful for clients to know what versions are supported.
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version"); // This determines how the API version will be read from the request.
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";	// This controls how the API version will be represented in the Swagger UI (or any other API documentation). The format 'v'V means that the version will be displayed as v1, v2, etc.
            options.SubstituteApiVersionInUrl = true; // Automatically substitutes the version in the URL
        });

        return services;
    }

}