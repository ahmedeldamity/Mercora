using Core.Entities.IdentityEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.ConfigurationData;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service;
public class AuthService(IOptions<JWTData> jWTData) : IAuthService
{
    private readonly JWTData _jWTData = jWTData.Value;

    public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
    {

        // Private Claims (user defined - can change from user to other)
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.GivenName, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var userRoles = await userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        // secret key
        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTData.SecretKey));

        // Token Object
        var token = new JwtSecurityToken(
            // Registered Claims
            issuer: _jWTData.ValidIssuer,
            audience: _jWTData.ValidAudience,
            expires: DateTime.UtcNow.AddMinutes(_jWTData.DurationInMinutes),
            // Private Claims
            claims: authClaims,
            // Signature Key
            signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
        );

        // Create Token And Return It
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}