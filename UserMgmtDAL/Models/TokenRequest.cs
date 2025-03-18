using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public class TokenRequest
    {
        public string RefreshToken { get; set; }
        public string userName { get; set; }

        public int userId { get; set; }

        public string struserGuId { get; set; }
    }


}
