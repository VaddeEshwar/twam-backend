using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserMgmtDAL.Models;

namespace UserMgmtDAL.Helpers
{
    public static class SecurityHelper
    {
        private static readonly int SaltSize = 16; // Size of the salt in bytes
                                                   //public static string GenerateJSONWebToken(RegisterUserRequest userInfo, IConfiguration _config)
                                                   //{
                                                   //    try
                                                   //    {
                                                   //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                                                   //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                                                   //        // Calculate expiration time
                                                   //        DateTime expiration = DateTime.UtcNow.AddMinutes(120);
                                                   //        var claims = new[]
                                                   //        {
                                                   //          new Claim(JwtRegisteredClaimNames.Name, userInfo.FirstName),
                                                   //    new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                                                   //    new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRole), userInfo.GroupId)), // Convert enum to role name // Standard claim type for role 
                                                   //   new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()), // Standard claim type for user ID
                                                   //    new Claim("DateOfJoing", userInfo.CreatedAt.ToString("yyyy-MM-dd")),
                                                   //    new Claim(JwtRegisteredClaimNames.Exp, ((DateTimeOffset)expiration).ToUnixTimeSeconds().ToString()), // Expiration claim
                                                   //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                                                   //    };

        //        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
        //            _config["Jwt:Audience"],
        //            claims,
        //            expires: DateTime.Now.AddMinutes(120),
        //            signingCredentials: credentials);

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public static string GenerateSalt(int length = 16)
        {
            var saltBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }


        //create has password

        public static (string hash, string salt) HashPassword(string password)
        {
            // Generate a random salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                var saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                var salt = Convert.ToBase64String(saltBytes);
                // Convert salt from base64 string to byte array
                var saltBytesAsBytes = Convert.FromBase64String(salt);
                // Concatenate password and salt
                var passwordWithSalt = password + salt;

                // Compute hash of password with salt
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                    var hash = Convert.ToBase64String(hashBytes);
                    return (hash, salt);
                }
            }
        }
            public static string  HashPassword1(string password)
        {
            const int SaltSize = 16; // Choose an appropriate salt size

            // Generate a random salt
            byte[] salt;
            using (var rng = new RNGCryptoServiceProvider())
            {
                salt = new byte[SaltSize];
                rng.GetBytes(salt);
            }

            // Create the salted hash
            string hashedPassword;
            using (var sha256Hash = SHA256.Create())
            {
                // Combine the password and salt
                byte[] combinedBytes = new byte[password.Length + SaltSize];
                Buffer.BlockCopy(Encoding.UTF8.GetBytes(password), 0, combinedBytes, 0, password.Length);
                Buffer.BlockCopy(salt, 0, combinedBytes, password.Length, SaltSize);

                // Compute the hash
                byte[] hashBytes = sha256Hash.ComputeHash(combinedBytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                hashedPassword = builder.ToString();
            }

            return hashedPassword;
        }

        public static void ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = null;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JWT token: {ex.Message}");
                // Handle token read errors
                return;
            }

            // Example: Retrieving user ID claim
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                // Process user ID
            }
            else
            {
                Console.WriteLine("User ID claim not found in token.");
                // Handle missing user ID claim
            }
        }

        // Verify Password
        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {

            // Combine input password and stored salt
            var passwordWithSalt = enteredPassword + storedSalt;

            // Hash the combined password and salt with SHA-256
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                var inputHash = Convert.ToBase64String(hashBytes);

                return inputHash == storedHash;
            }
            
        }

        public static string GetRootDomain(HttpRequest request)
        {
            // Get the scheme (http or https)
            string scheme = request.Scheme;

            // Get the host (domain name)
            string host = request.Host.Value;

            // Construct and return the root domain URL
            return $"{scheme}://{host}";
        }
    }

}
