using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Repositories.Abstract
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateAccessToken(string username);
        DateTime GetExpiryDate();
        //Task StoreRefreshTokenAsync(int userId, string refreshToken);
    }
}
