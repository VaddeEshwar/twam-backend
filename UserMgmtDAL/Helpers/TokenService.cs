using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Repositories.Abstract;

public class TokenService : ITokenService
{
    private readonly TokenOptions _tokenOptions;

    public TokenService(TokenOptions tokenOptions)//TokenOptions tokenOptions
    {
        _tokenOptions = tokenOptions;
    }

    public string Keygenrator()
    {
        // Generate a 256-bit (32-byte) client secret key
        byte[] key = new byte[32];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(key);
        }

        // Convert the key to a base64 string for storage or display
        string base64Key = Convert.ToBase64String(key);
        return base64Key;
        // Console.WriteLine($"Generated Client Secret Key (Base64): {base64Key}");
    }
    public string GenerateAccessToken(string userGUId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userGUId)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    public  DateTime GetExpiryDate()
    {
        int expiryInMinutes =Convert.ToInt32(_tokenOptions.RefreshTokenExpiration);
        return DateTime.UtcNow.AddMinutes(expiryInMinutes);
    }
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _tokenOptions.Issuer,
            ValidAudience = _tokenOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey))
        }, out var securityToken);

        return principal;
    }
}
