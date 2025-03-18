using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgmtDAL.Models;

namespace UserMgmtDAL.Repositories.Abstract
{
    public interface IrefreshToken
    {
        Task InsertRefreshTokenAsync(int userId, string refreshToken);
        Task<string> GetUserIdFromRefreshTokenAsync(string refreshToken);
        Task InsertRefreshTokenAsync(RefreshToken objrefreshToken);
        Task<string> GetStoredRefreshTokenAsync(string userGuid);
    }
}
