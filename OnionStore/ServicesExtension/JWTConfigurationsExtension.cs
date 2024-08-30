using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.ConfigurationData;
using System.Text;

namespace API.ServicesExtension;
public static class JWTConfigurationsExtension
{
    public static IServiceCollection AddJWTConfigurations(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var jwtData = serviceProvider.GetRequiredService<IOptions<JWTData>>().Value;

        // AddAuthentication() : this method take one argument (Default Schema)
        // and when we using .AddJwtBearer(): this method can take from you another schema and options
        // and can take just options and this options worked on the default schema that you written it in AddAuthentication()
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // We use it for to be don't have to let every end point what is the shema because it will make every end point work on bearer schema

        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = jwtData.ValidAudience,
                ValidateIssuer = true,
                ValidIssuer = jwtData.ValidIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtData.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
        })
        // If You need to doing some options on another schema
        .AddJwtBearer("Bearer2", options =>
        {
        
        });

        return services;
    }

}