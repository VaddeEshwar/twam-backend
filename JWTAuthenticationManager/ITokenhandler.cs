using JWTAuthenticationManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthenticationManager
{
    public interface ITokenhandler
    {
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        AuthenticationResponse GenerateAccessToken(string username);
        DateTime GetExpiryDate();
    }
}
