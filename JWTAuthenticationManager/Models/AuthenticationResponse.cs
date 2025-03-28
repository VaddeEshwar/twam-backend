﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthenticationManager.Models
{
    public class AuthenticationResponse
    {
        public string userName { get; set; }
        public string JwtToken { get; set; }
        public int ExpiresIn { get; set; }

        public string userGUID { get; set; }
    }
}
