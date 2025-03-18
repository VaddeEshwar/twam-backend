using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public class Authentication
    {
    }
    public class RegisterUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string CountryCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int role { get; set; }
        public int Status { get; set; }
        public string usrStatus { get; set; }
        public int userId { get; set; }
        public string PasswordHash { get; set; }
        public string salt { get; set; }
        public int emalverified { get; set; }
        public string Token { get; set; }
        public string gender {  get; set; }

        public DateTime dateofBirth { get; set; }
        public string verificationURL { get; set; }
        public string strApp { get; set; }
        public string userGuid { get; set; }
    }
    public class userAddress
    {
        public string GuserId { get; set; }
        public int userId { get; set; }
        public int addressId { get; set; }
        public int userAddressId { get; set; }
        public int userAddressstatus { get; set; }
        public string strMsg { get; set; }
        public string strApp { get; set; }
    }
    public class Address
    {

        public int AddressID { get; set; }
        public string HouseNumber { get; set; }
        public string BuildingName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int CityID { get; set; }
        public int DistrictID { get; set; }
        public int StateID { get; set; }
        public int CountrID { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; }
        public int AddressType { get; set; }
        public bool IsaddressUpdate { get; set; }
    }
        public class UpdateuserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string CountryCode { get; set; }
        public DateTime dateofBirth { get; set; }
        public string userGuid { get; set; }
        public string gender { get; set; }       
        public List<Address> objAddress { get; set; }

    }
        public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        //public string strApp { get; set; }
        //public string userGUID { get; set; }
    }

    public class registeroutval
    {
        public string tokenStatus { get; set; }
        public int userId { get; set; }
       public  Guid verificationGuid { get; set; }
    }
    public class AuthResponse
    {
        public string userName { get; set; }
        public string JwtToken { get; set; }
        public int ExpiresIn { get; set; }

        public string userGUID { get; set; }
    }

    public class JwtResponse
    {
        public string Token { get; set; }
    }

}
