using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthenticationManager
{
    public class UserAccount
    {
        public string userName {  get; set; }
        public string password { get; set; }

        public string Role { get; set; }
    }
}
