using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthenticationManager.Models
{
    public class AuthenticationRequest
    {
        public string userName { get; set; }
        public string password { get; set; }

        public string userGUID { get; set; }
    }
}
