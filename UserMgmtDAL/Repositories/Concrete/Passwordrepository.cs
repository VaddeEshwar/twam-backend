using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Repositories.Abstract;

namespace UserMgmtDAL.Repositories.Concrete
{
    public class Passwordrepository : Ipassword
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Passwordrepository(IHttpContextAccessor httpContextAccessor)
        {
            // _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string Status, Guid UserGUID)> ChangePasswordAsync(string email, string newPassword, string salt)
        {
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string connectionString = SqlHelper.GetConnectiondetails(appType1);
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("ChangePassword", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@NewPassword", newPassword);
                    command.Parameters.AddWithValue("@Salt", salt);

                    var statusParameter = new SqlParameter("@Status", SqlDbType.NVarChar)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var userGuidParameter = new SqlParameter("@UserGUID", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.Parameters.Add(statusParameter);
                    command.Parameters.Add(userGuidParameter);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    string status = (String)statusParameter.Value;
                    Guid userGuid = (Guid)userGuidParameter.Value;
                    //if (userGuid != Guid.Empty)
                    //{
                    //    UserActivityData userActivityData = new UserActivityData();
                    //    userActivityData.UserGuid = userGuid.ToString();
                    //    userActivityData.UserActivity = "ChangePassword";
                    //    userActivityData.description = Const.Changed_Password;
                    //    Useractivityhistory useractivityhistory = new Useractivityhistory(_httpContextAccessor);
                    //    useractivityhistory.InsertUserActivityAsync(userActivityData);
                    //}

                    return (status, userGuid);
                }
            }
            catch (Exception ex)
            {
                
                // Log the exception
                // _logger.LogError(ex, "An error occurred while changing the password.");

                // Return error status
                return ("An error occurred while changing the password", Guid.Empty);
            }
            
        }

        public async Task<(string Passcode, DateTime Expiry, int Result, string FirstName,string strGuId)> GenerateOtpAsync(string email)
        {
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("GenerateAndInsertOTP", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Email", email);

                var passcodeParameter = new SqlParameter("@Passcode", SqlDbType.Char, 6)
                {
                    Direction = ParameterDirection.Output
                };
                var expiryParameter = new SqlParameter("@Expiry", SqlDbType.DateTime)
                {
                    Direction = ParameterDirection.Output
                };
                var resultParameter = new SqlParameter("@Result", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var firstNameParameter = new SqlParameter("@FirstName", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                var userGuidParameter = new SqlParameter("@UserGUID", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };
            
                command.Parameters.Add(userGuidParameter);
                command.Parameters.Add(passcodeParameter);
                command.Parameters.Add(expiryParameter);
                command.Parameters.Add(resultParameter);
                command.Parameters.Add(firstNameParameter);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                string passcode = (string)passcodeParameter.Value;
                DateTime expiry = (DateTime)expiryParameter.Value;
                int result = (int)resultParameter.Value;
                string firstName = (string)firstNameParameter.Value;
                string strGuId = (string)userGuidParameter.Value;

                return (passcode, expiry, result, firstName,strGuId);
            }
        }
    }
}
