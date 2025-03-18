using UserMgmtDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Repositories.Abstract
{
    public interface IRolerepository
    {
         Task<List<RoleDto>> GetRolesAsync();
    }
}
