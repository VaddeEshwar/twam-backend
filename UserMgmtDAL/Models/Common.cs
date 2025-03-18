using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{

    public class Country
    {

        public int? CountryID { get; set; }
        public string? CountryName { get; set; }
        public string? CountryCode { get; set; }
        public int? Dial { get; set; }
        public string? Currency_Name { get; set; }
        public string? Currency { get; set; }
        public bool? IsActive { get; set; }
    }
    public class State
    {

        public int? StateID { get; set; }
        public string? StateName { get; set; }
        public int? CountryID { get; set; }
    }
    public class City
    {

        public int? CityID { get; set; }
        public string? CityName { get; set; }
        public int? StateID { get; set; }
    }
}
