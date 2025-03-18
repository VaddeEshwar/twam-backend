using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;

namespace UserMgmtDAL.Repositories.Concrete
{
    public class Rolesrepository : IRolerepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Rolesrepository(IHttpContextAccessor httpcontext)
        {

             _httpContextAccessor = httpcontext;
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            List<RoleDto> roles = new List<RoleDto>();
            //var roles = new List<RoleDto>();
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetStatusType", connection))
                {
                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int intstatusId = Convert.ToInt32(reader["StatusId"].ToString());
                            string strStatusName = reader["StatusName"].ToString();
                            string strStatusType = reader["StatusType"].ToString();
                            string strstatusdes =reader["StatusDescrition"].ToString();
                            roles.Add(new RoleDto
                            {
                                StatusId = intstatusId,
                                StatusName = strStatusName,
                                StatusType= strStatusType,
                                StatusDescrition=strstatusdes
                            });
                        }
                    }
                }
            }

            return roles;
        }

    }
}
