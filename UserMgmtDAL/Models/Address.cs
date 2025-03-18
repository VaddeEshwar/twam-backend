using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public class Addressdetails
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

        public int ResultAddressId { get; set; }
        public string SuccessMessage { get; set; }
        public string userGuId { get; set; }
        public int userId { get; set; }
        public int userAddressId { get; set; }
        public string strApp { get; set; }
        //public userAddress objuserAddress { get; set; }

    }
  

    public class Addressreturnval
    {
        public string strMsg { get; set; }
        public bool sucessval { get; set; }
        public int addressId { get; set; }
    }
    public class userAddressdetails
    {
        public string GuserId { get; set; }
        public int userId { get; set; }
        public int addressId { get; set; }
        public int userAddressId { get; set; }
        public string strMsg { get; set; }
        public string strApp { get; set; }
    }
}
