using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public  class UserActivity
    {
        public class UserActivityData
        {
            public int UserActivityId { get; set; }
            public string UserGuid { get; set; }
            public string UserActivity { get; set; }
            public DateTime CreatedOn { get; set; }
            public string Ipaddress { get; set; }
            public string macaddress { get; set; }

            public string description { get; set; }

            public int userId { get; set; }

        }

        public class UserActivityinputparms
        {
          
            public string UserGuid { get; set; }
            public string? UserActivity { get; set; }
            public DateTime? CreatedOn { get; set; }
            public DateTime? Fromdate { get; set; }
            public DateTime? Todate { get; set; }
        }
    }
}
