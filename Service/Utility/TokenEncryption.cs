using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Service.Utility;
public static class TokenEncryption
{
    public static readonly RSA _rsa;
    public static readonly RsaSecurityKey _rsaKey;

    static TokenEncryption()
    {
        _rsa = RSA.Create(2048);
        _rsaKey = new RsaSecurityKey(_rsa) { KeyId = "this is my custom Secret key for encryption" };
    }
}