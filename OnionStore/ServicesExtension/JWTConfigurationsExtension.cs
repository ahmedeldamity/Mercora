using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.ConfigurationData;
using System.Text;

namespace API.ServicesExtension
{
    public static class JWTConfigurationsExtension
    {
        public static IServiceCollection AddJWTConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var jwtData = serviceProvider.GetRequiredService<IOptions<JWTData>>().Value;
            var googleData = serviceProvider.GetRequiredService<IOptions<GoogleData>>().Value;

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
                    ClockSkew = TimeSpan.FromMinutes(jwtData.DurationInMinutes),
                };
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = googleData.ClientId;
                options.ClientSecret = googleData.ClientSecret;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("JwtPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy("GooglePolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(GoogleDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();

            return services;
        }
    }
}