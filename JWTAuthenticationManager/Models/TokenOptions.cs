﻿namespace JWTAuthenticationManager.Models
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
