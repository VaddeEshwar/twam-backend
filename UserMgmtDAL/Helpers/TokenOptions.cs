using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Helpers
{
    public class TokenOptions
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiration { get; set; } // in minutesAccessTokenExpiration
        public int RefreshTokenExpiration { get; set; } // in days
    }
}
