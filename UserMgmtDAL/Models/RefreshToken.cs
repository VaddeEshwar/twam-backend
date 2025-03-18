using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public class RefreshToken
    {
        public string userGUID { get; set; }
        public string refreshToken { get; set; }
        public DateTime? expiryDate { get; set; }
      
    }
}
