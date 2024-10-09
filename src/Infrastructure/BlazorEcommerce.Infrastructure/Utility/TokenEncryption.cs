using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace BlazorEcommerce.Infrastructure.Utility;
public static class TokenEncryption
{
    public static readonly RSA Rsa;
    public static readonly RsaSecurityKey RsaKey;

    static TokenEncryption()
    {
        Rsa = RSA.Create(2048);
        RsaKey = new RsaSecurityKey(Rsa) { KeyId = "this is my custom Secret key for encryption" };
    }
}