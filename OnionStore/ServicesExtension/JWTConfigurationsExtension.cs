using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service.ConfigurationData;
using Service.Utility;
using System.Text;

namespace API.ServicesExtension;
public static class JwtConfigurationsExtension
{
    public static IServiceCollection AddJwtConfigurations(this IServiceCollection services, JwtData jWtData)
    {
        // AddAuthentication() : this method take one argument (Default Schema)
        // and when we using .AddJwtBearer(): this method can take from you another schema and options
        // and can take just options and this options worked on the default schema that you have written it in AddAuthentication()
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
                ValidAudience = jWtData.ValidAudience,
                ValidateIssuer = true,
                ValidIssuer = jWtData.ValidIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWtData.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                TokenDecryptionKey = TokenEncryption.RsaKey
            };
        })
        // If You need to doing some options on another schema
        .AddJwtBearer("Bearer2", options =>
        {
        
        });

        return services;
    }

}