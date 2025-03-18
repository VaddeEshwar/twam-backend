using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string Salt { get; set; }
    }
    public class GenerateOtpRequest
    {
        public string Email { get; set; }
    }
}
