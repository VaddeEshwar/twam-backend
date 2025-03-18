using UserMgmtDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Repositories.Abstract
{
    public interface IAuthService
    {
        Task<JwtResponse> AuthenticateAsync(LoginRequest request);
        Task<User> RegisterUserAsync(RegisterUserRequest request);
    }

}
