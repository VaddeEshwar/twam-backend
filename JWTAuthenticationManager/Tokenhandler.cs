using JWTAuthenticationManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserMgmtDAL;
using UserMgmtDAL.Helpers;
using TokenOptions = UserMgmtDAL.Helpers.TokenOptions;

namespace JWTAuthenticationManager
{
    public class Tokenhandler : ITokenhandler
    {
        public const string Jwt_secret_key = "rqjBPV0WmThOo7SJqJTum+x1wzrX3NMeLlB3wgWYdWE=";
        public int Jwt_Token_Expiry = 20;
        private readonly TokenOptions _tokenOptions;       
        public Tokenhandler(TokenOptions tokenOptions)
        {
            _tokenOptions = tokenOptions;
        }
      
       
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public DateTime GetExpiryDate()
        {
            int expiryInMinutes = Convert.ToInt32(_tokenOptions.RefreshTokenExpiration);
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
        public AuthenticationResponse GenerateAccessToken(string userGUID)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, userGUID)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt_secret_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var TokenExpiryTimestamp = DateTime.Now.AddMinutes(Jwt_Token_Expiry); //DateTime.UtcNow.AddMinutes(Jwt_Token_Expiry);
            var token = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer ,//"https://localhost:7074/",
               audience: _tokenOptions.Audience,// "https://localhost:7074/",
                claims: claims,
                expires: TokenExpiryTimestamp,//_tokenOptions.AccessTokenExpiration
                signingCredentials: creds);
            var JwtsecurityTokenhandler = new JwtSecurityTokenHandler();
            var Tokenkey = JwtsecurityTokenhandler.WriteToken(token);
            return new AuthenticationResponse
            {
                userGUID = userGUID,
                ExpiresIn = (int)TokenExpiryTimestamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = Tokenkey
            };
        }

        
    }


}
