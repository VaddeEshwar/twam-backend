using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Repositories.Abstract
{
    public interface Ipassword
    {
        Task<(string Status, Guid UserGUID)> ChangePasswordAsync(string email, string newPassword, string salt);
        Task<(string Passcode, DateTime Expiry, int Result, string FirstName,string strGuId)> GenerateOtpAsync(string email);
    }
}
