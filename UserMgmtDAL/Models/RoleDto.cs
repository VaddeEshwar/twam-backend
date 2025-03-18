using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public class RoleDto
    {
        public int  StatusId { get; set; }
        public string StatusName { get; set; }
       public string StatusType {  get; set; }
        public string   StatusDescrition { get; set; }
        public string strApp { get; set; }

    }
}
