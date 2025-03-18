using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using UserMgmtDAL.Constants;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;
using static UserMgmtDAL.Models.UserActivity;

namespace UserMgmtDAL.Repositories.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserRepository(IHttpContextAccessor httpContextAccessor)
        {
            // _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<RegisterUserRequest> GetUserByUsernameAsync(LoginRequest request)
        {
            // string connectionString = SqlHelper.Getconnection(request.strApp);
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string connectionString = SqlHelper.GetConnectiondetails(appType1);
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var query = "Get_LoginUser";

                    using (var command = new SqlCommand("Get_LoginUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@email", request.Username);

                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            if (await reader.ReadAsync())
                            {
                                int userId1 = reader.GetInt32("UserId");
                                string text = reader["Password_Hash"] as string;
                                return new RegisterUserRequest
                                {
                                    userId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    userGuid = reader.GetString(reader.GetOrdinal("User_Guid")),
                                    FirstName = reader.GetString(reader.GetOrdinal("First_Name")),
                                    LastName = reader.GetString(reader.GetOrdinal("Last_Name")),
                                    PasswordHash = reader["Password_Hash"] as string,
                                    salt = reader["Salt"] as string,
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    emalverified = reader.GetInt32(reader.GetOrdinal("user_verified"))
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                // Log SQL exceptions here
                throw new ApplicationException("Database error occurred.", ex);
            }
            catch (Exception ex)
            {
                // Log other exceptions here
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<registeroutval> RegisterUserAsync(RegisterUserRequest request)
        {
            string verificationGuid;
            int userId; string strtatus;
            var (hashpwd, saltval) = SecurityHelper.HashPassword(request.Password);

            string strhashpwd = SecurityHelper.HashPassword(request.Password).ToString();
            // string []strhashpwdarray = hashpwd.Split(",");
            // string strhashpwd = strhashpwdarray[0];
            registeroutval objregisteroutval = new registeroutval();
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("Register_User", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@First_Name", request.FirstName);
                        command.Parameters.AddWithValue("@Last_Name", request.LastName);
                        command.Parameters.AddWithValue("@Mobile", request.Mobile);
                        command.Parameters.AddWithValue("@Dial", request.CountryCode);
                        command.Parameters.AddWithValue("@Password_Hash", hashpwd);
                        command.Parameters.AddWithValue("@Salt", saltval);
                        command.Parameters.AddWithValue("@Email", request.Email);
                        command.Parameters.AddWithValue("@Token", request.Token);
                        command.Parameters.AddWithValue("@Verification_url", request.verificationURL);
                        command.Parameters.AddWithValue("@Role", request.role);
                        command.Parameters.AddWithValue("@UserStatus", 0);
                        command.Parameters.Add("@VerificationGUID", SqlDbType.Char, 100).Direction = ParameterDirection.Output;

                        command.Parameters.Add("@UserId", SqlDbType.Int).Direction = ParameterDirection.Output;
                        SqlParameter outputIdParam = new SqlParameter("@Status", SqlDbType.VarChar, 100);
                        outputIdParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputIdParam);
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        userId = (int)command.Parameters["@UserId"].Value;
                        strtatus = command.Parameters["@Status"].Value.ToString();
                        verificationGuid = command.Parameters["@VerificationGUID"].Value.ToString().Trim();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log SQL exceptions here
                throw new ApplicationException("Database error occurred.", ex);
            }
            catch (Exception ex)
            {
                // Log other exceptions here
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
            objregisteroutval.tokenStatus = strtatus;
            objregisteroutval.userId = userId;
            objregisteroutval.verificationGuid = Guid.Parse(verificationGuid);
            return (objregisteroutval);
        }

        public async Task<registeroutval> VerifyEmailAsync(Guid verificationGuid)
        {
            // var appType = _httpContextAccessor.HttpContext.Items["AppType"] as string;


            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            registeroutval objregisteroutval = new registeroutval();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "VerifyEmail";//"UPDATE Users SET IsEmailVerified = 1 WHERE VerificationGUID = @Guid AND IsEmailVerified = 0";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@token", verificationGuid);
                        command.Parameters.Add("@status", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                        command.Parameters.Add("@userId", SqlDbType.Int).Direction = ParameterDirection.Output;

                        await connection.OpenAsync();
                        var rowsAffected = await command.ExecuteNonQueryAsync();


                        int userId = (int)command.Parameters["@userId"].Value;
                        string strtatus = command.Parameters["@status"].Value.ToString();
                        objregisteroutval.userId = userId;
                        objregisteroutval.tokenStatus = strtatus;


                        return objregisteroutval;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log SQL exceptions here
                objregisteroutval.tokenStatus = ex.Message;
                objregisteroutval.userId = 0;
                return objregisteroutval;
                //  throw new ApplicationException("Database error occurred.", ex);
            }
            catch (Exception ex)
            {
                // Log other exceptions here
                objregisteroutval.tokenStatus = ex.Message;
                objregisteroutval.userId = 0;
                return objregisteroutval;
                //throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> IsEmailVerifiedAsync(string email)
        {
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND IsEmailVerified = 1";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        await connection.OpenAsync();
                        var count = (int)await command.ExecuteScalarAsync();

                        return count > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log SQL exceptions here
                throw new ApplicationException("Database error occurred.", ex);
            }
            catch (Exception ex)
            {
                // Log other exceptions here
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }


        public async Task<string> UpdateUserAsync(UpdateuserRequest registerUserRequest)
        {
            string status = string.Empty;
            Addressreturnval addressval = new Addressreturnval();
            UserActivityData userActivityData = new UserActivityData();
            userAddress objuserAddressdetails = new userAddress();
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand("UpdateUser", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@User_Guid", registerUserRequest.userGuid);
                            command.Parameters.AddWithValue("@firstname", (object)registerUserRequest.FirstName ?? DBNull.Value);
                            command.Parameters.AddWithValue("@lastname", (object)registerUserRequest.LastName ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Gender", (object)registerUserRequest.gender ?? DBNull.Value);
                            command.Parameters.AddWithValue("@mobile", (object)registerUserRequest.Mobile ?? DBNull.Value);
                            command.Parameters.AddWithValue("@countrycode", (object)registerUserRequest.CountryCode ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Date_Of_Birth", (object)registerUserRequest.dateofBirth ?? DBNull.Value);

                            var outputParam = new SqlParameter("@Status", SqlDbType.VarChar, 100)
                            {
                                Direction = ParameterDirection.Output
                            };
                            command.Parameters.Add(outputParam);

                            await command.ExecuteNonQueryAsync();
                            status = (string)outputParam.Value;

                            if (status == "1")
                            {
                                //if (registerUserRequest.objAddress != null && registerUserRequest.objAddress.Count > 0 && registerUserRequest.objAddress[0].IsaddressUpdate)
                                if (registerUserRequest.objAddress[0].IsaddressUpdate == true)
                                {
                                    addressval = await InsertAddressAsync(registerUserRequest.objAddress);
                                    if (addressval.addressId != 0)
                                    {
                                        objuserAddressdetails.addressId = addressval.addressId;
                                        objuserAddressdetails.GuserId = registerUserRequest.userGuid;
                                        //objuserAddressdetails.userAddressId = addressval.addressId;

                                        objuserAddressdetails = await InsertuserAddress(objuserAddressdetails);
                                        if (objuserAddressdetails.userAddressId != 0 && (objuserAddressdetails.userAddressstatus == 1 || objuserAddressdetails.userAddressstatus == 2))
                                        {
                                            status = Const.User_Updated;
                                        }
                                    }
                                }
                                transaction.Commit();
                            }

                            // Log user activity if necessary
                            if (!string.IsNullOrEmpty(registerUserRequest.userGuid) && (objuserAddressdetails.userAddressstatus == 1 || objuserAddressdetails.userAddressstatus == 2))
                            {
                                userActivityData.UserGuid = registerUserRequest.userGuid;
                                userActivityData.UserActivity = "Update User";
                                userActivityData.description = Const.User_Updated;

                                Useractivityhistory useractivityhistory = new Useractivityhistory(_httpContextAccessor);
                                await useractivityhistory.InsertUserActivityAsync(userActivityData);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback on error
                        transaction.Rollback();
                        // Log the exception (consider using a logging framework)
                        status = "Error: " + ex.Message; // Or a more appropriate error status
                    }
                }
            }
            return status;
        }



        public async Task<bool> ValidateOTP(string email, string passcode)
        {
            bool isValid = false;

            try
            {
                var appType1 = SqlHelper.GetAppType(_httpContextAccessor);
                // var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
                string _connectionString = SqlHelper.GetConnectiondetails(appType1);

                using (var connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("ValidateOTP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 50) { Value = email });
                        command.Parameters.Add(new SqlParameter("@EnteredPasscode", SqlDbType.VarChar, 8) { Value = passcode });

                        // Output parameter
                        var isValidParam = new SqlParameter("@IsValid", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                        command.Parameters.Add(isValidParam);

                        // Open the connection and execute the command
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();

                        // Retrieve output parameter value
                        isValid = Convert.ToBoolean(isValidParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return isValid;
        }





        public async Task<Addressreturnval> InsertAddressAsync(List<Address> ojaddress)
        {
            Addressreturnval objAddressreturnval = new Addressreturnval();
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync() as SqlTransaction)
                {
                    try
                    {
                        foreach (var item in ojaddress)
                        {
                            using (var command = new SqlCommand("UpsertAddress", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@AddressID", item.AddressID);
                                command.Parameters.AddWithValue("@HouseNumber", item.HouseNumber ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@BuildingName", item.BuildingName ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@AddressLine1", item.AddressLine1 ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@AddressLine2", item.AddressLine2 ?? (object)DBNull.Value);

                                command.Parameters.AddWithValue("@CityID", item.CityID == 0 ? 0 : (object)item.CityID);
                                command.Parameters.AddWithValue("@DistrictID", item.DistrictID == 0 ? 0 : (object)item.DistrictID);
                                command.Parameters.AddWithValue("@StateID", item.StateID == 0 ? 0 : (object)item.StateID);
                                command.Parameters.AddWithValue("@CountryID", item.CountrID == 0 ? 0 : (object)item.CountrID);
                                command.Parameters.AddWithValue("@ZipCode", item.ZipCode ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@AddressType", item.AddressType == 0 ? 0 : (object)item.AddressType);

                                var outputAddressId = new SqlParameter("@ResultAddressID", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                command.Parameters.Add(outputAddressId);

                                var outputMessage = new SqlParameter("@SuccessMessage", SqlDbType.NVarChar, 100)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                command.Parameters.Add(outputMessage);

                                // Execute the command
                                await command.ExecuteNonQueryAsync();

                                // Capture output values
                                objAddressreturnval.addressId = (int)outputAddressId.Value;
                                objAddressreturnval.strMsg = outputMessage.Value.ToString();
                                objAddressreturnval.sucessval = true; // Indicate success for this operation
                            }
                        }

                        // Commit the transaction after all commands succeed
                        await transaction.CommitAsync();
                    }
                    catch (SqlException ex)
                    {
                        await transaction.RollbackAsync();
                        objAddressreturnval.sucessval = false;
                        objAddressreturnval.strMsg = "Database error occurred: " + ex.Message;
                        // Log SQL exceptions here
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        objAddressreturnval.sucessval = false;
                        objAddressreturnval.strMsg = "An unexpected error occurred: " + ex.Message;
                        // Log other exceptions here
                    }
                }
            }

            return objAddressreturnval;
        }


        public async Task<userAddress> InsertuserAddress(userAddress ojaddress)
        {
            userAddress objAddressreturnval = new userAddress();
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("UpsertUserAddress", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@UserId", ojaddress.GuserId);
                        command.Parameters.AddWithValue("@AddressId", ojaddress.addressId);

                        // If userAddressId is set, use it; otherwise, set it to 0
                        command.Parameters.AddWithValue("@UserAddressId", ojaddress.userAddressId != 0 ? ojaddress.userAddressId : (object)DBNull.Value);

                        // Output parameter for success message
                        var successMessageParam = new SqlParameter("@SuccessMessage", SqlDbType.Int, 100)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(successMessageParam);

                        // Execute the command
                        await command.ExecuteNonQueryAsync();

                        // Read the output values if necessary
                        //objAddressreturnval.userAddressId = (int)(command.Parameters["@UserAddressId"].Value ?? 0);
                        objAddressreturnval.userAddressstatus =Convert.ToInt32(successMessageParam.Value);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                objAddressreturnval.userAddressId = reader.GetInt32(reader.GetOrdinal("UserAddressId")); // Adjust based on your column name
                                                                                                                         // Populate other fields if needed
                            }
                        }

                        // You may want to return additional information, or you could modify
                        // the stored procedure to return the user ID and address ID if needed.
                        return objAddressreturnval;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log SQL exceptions here
                throw new ApplicationException("Database error occurred.", ex);
            }
            catch (Exception ex)
            {
                // Log other exceptions here
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }


        public async Task<string> GetDatabsucces()
        {

            string success = string.Empty;
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    success = (connection.State == ConnectionState.Open) ? "Pass" : "Fail";
                }
            }

            catch (Exception ex)
            {
                return success = "Fail";
                throw ex.InnerException;
            }

            return await Task.FromResult(success);
        }

    }
}
