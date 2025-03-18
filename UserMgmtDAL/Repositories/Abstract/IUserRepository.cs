using UserMgmtDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Repositories.Abstract
{

    public interface IUserRepository
    {
        Task<RegisterUserRequest> GetUserByUsernameAsync(LoginRequest request);
        Task<registeroutval> RegisterUserAsync(RegisterUserRequest user);
        Task<registeroutval> VerifyEmailAsync(Guid verificationGuid);
        Task<bool> ValidateOTP(string email, string passcode);
        Task<string> UpdateUserAsync(UpdateuserRequest registerUserRequest);
     
        Task<string> GetDatabsucces();
     
        //Task<bool> IsEmailVerifiedAsync(string email);
    }

}
