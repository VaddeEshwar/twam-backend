using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgmtDAL.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using System.Transactions;
using System.Data.Common;
using UserMgmtDAL.Constants;
using static UserMgmtDAL.Models.UserActivity;

namespace UserMgmtDAL.Repositories.Concrete
{
    public class addressRepository : Iaddress
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public addressRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        // string strconnection = SqlHelper.Getconnection();
        //public async Task<Addressreturnval> InsertAddressAsync(Address ojaddress)
        //{
        //    //string strconnection = SqlHelper.Getconnection("");
        //    Guid verificationGuid;
        //    int resultAddressId = 0; string statusMsg = null; bool userreturnval = false;

        //    Addressreturnval objAddressreturnval = new Addressreturnval();
        //    var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
        //    string _connectionString = SqlHelper.GetConnectiondetails(appType1);

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                using (var command = new SqlCommand("UpsertAddress", connection))

        //                {
        //                    command.CommandType = CommandType.StoredProcedure;
        //                    command.Parameters.AddWithValue("@AddressID", ojaddress.AddressID);
        //                    command.Parameters.AddWithValue("@HouseNumber", ojaddress.HouseNumber);
        //                    command.Parameters.AddWithValue("@BuildingName", ojaddress.BuildingName);
        //                    command.Parameters.AddWithValue("@AddressLine1", ojaddress.AddressLine1);
        //                    command.Parameters.AddWithValue("@AddressLine2", ojaddress.AddressLine2);
        //                    command.Parameters.AddWithValue("@CityID", ojaddress.CityID);
        //                    command.Parameters.AddWithValue("@DistrictID", ojaddress.DistrictID);
        //                    command.Parameters.AddWithValue("@StateID", ojaddress.StateID);
        //                    command.Parameters.AddWithValue("@CountryID", ojaddress.CountrID);
        //                    command.Parameters.AddWithValue("@ZipCode", ojaddress.ZipCode);
        //                    command.Parameters.AddWithValue("@AddressType", ojaddress.AddressType);

        //                    command.Parameters.Add("@ResultAddressID", SqlDbType.Int).Direction = ParameterDirection.Output;
        //                    command.Parameters.Add("@SuccessMessage", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
        //                    await connection.OpenAsync();
        //                    await command.ExecuteNonQueryAsync();
        //                    resultAddressId = (int)command.Parameters["@ResultAddressID"].Value;
        //                    statusMsg = command.Parameters["@SuccessMessage"].Value.ToString();
        //                }

        //                if (resultAddressId != 0)
        //                {
        //                    userAddress objuserAddress = new userAddress();
        //                    objuserAddress.addressId = resultAddressId;// addressDto.AddressID;
        //                    objuserAddress.GuserId = ojaddress.userGuId;
        //                    if (ojaddress.userAddressId != 0)
        //                    {
        //                        objuserAddress.userAddressId = ojaddress.userAddressId;
        //                    }
        //                    else
        //                        objuserAddress.userAddressId = 0;
        //                    objuserAddress = await InsertuserAddress(objuserAddress);
        //                    if (objuserAddress.userAddressId != 0)
        //                    {
                               
        //                        objAddressreturnval.sucessval = true;
        //                        objAddressreturnval.strMsg =Const.Updateaddress_Successfully;
        //                        transaction.Commit();
        //                    }
        //                    else
        //                    {
        //                        objAddressreturnval.sucessval = false;
        //                        objAddressreturnval.strMsg = Const.Addressupdare_notSuccessfully;
        //                        transaction.Rollback();
        //                    }

        //                }
                        
        //            }
        //            catch (SqlException ex)
        //            {
        //                transaction.Rollback();
        //                objAddressreturnval.sucessval = false;
        //                objAddressreturnval.strMsg = "Database error occurred. " +ex.Message.ToString(); 
        //                // Log SQL exceptions here
        //                //throw new ApplicationException("Database error occurred.", ex);
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                objAddressreturnval.sucessval = false;
        //                objAddressreturnval.strMsg = "An unexpected error occurred.  " + ex.Message.ToString(); 
        //                // Log other exceptions here
        //               // throw new ApplicationException("An unexpected error occurred.", ex);
        //            }
        //        }
        //        if (ojaddress.userId != 0)
        //        {
        //            UserActivityData userActivityData = new UserActivityData();
        //            userActivityData.userId = ojaddress.userId;
        //            userActivityData.UserActivity = "Update/Insert Address";
        //            userActivityData.description = objAddressreturnval.strMsg;
        //            Useractivityhistory useractivityhistory = new Useractivityhistory(_httpContextAccessor);
        //            useractivityhistory.InsertUserActivityAsync(userActivityData);

        //        }
        //    }
        //    return objAddressreturnval;
        //}

        //public async Task<userAddress> InsertuserAddress(userAddress ojaddress)
        //{

        //    Guid verificationGuid;
        //    int resultAddressId = 0; string statusMsg = null;

        //    userAddress objAddressreturnval = new userAddress();
        //    var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
        //    string _connectionString = SqlHelper.GetConnectiondetails(appType1);
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            using (var command = new SqlCommand("UpsertUserAddress", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@UserId", ojaddress.GuserId);
        //                command.Parameters.AddWithValue("@AddressId", ojaddress.addressId);
        //                command.Parameters.AddWithValue("@UserAddressId", ojaddress.userAddressId);
        //                //command.Parameters.Add("@StsMessage", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
        //                // command.Parameters.AddWithValue("@UserAddressId", ojaddress.userAddressId);
        //                await connection.OpenAsync();
        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    //string strstatusMsg = command.Parameters["@StsMessage"].Value.ToString();
        //                    // Read the output record
        //                    if (await reader.ReadAsync())
        //                    {
        //                        int intuserId = Convert.ToInt32(reader["UserId"].ToString());
        //                        int intaddressId = Convert.ToInt32(reader["AddressId"].ToString());
        //                        int intUserAddressId = Convert.ToInt32(reader["UserAddressId"].ToString());



        //                        return new userAddress
        //                        {
        //                            userId = intuserId,
        //                            addressId = intaddressId,
        //                            userAddressId = intUserAddressId,
        //                            // strMsg = strstatusMsg
        //                        };
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Log SQL exceptions here
        //        throw new ApplicationException("Database error occurred.", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log other exceptions here
        //        throw new ApplicationException("An unexpected error occurred.", ex);
        //    }


        //    return (objAddressreturnval);

        //}


    }
}
