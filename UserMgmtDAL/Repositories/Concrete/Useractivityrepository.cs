using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgmtDAL.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using static UserMgmtDAL.Models.UserActivity;

namespace UserMgmtDAL.Repositories.Concrete
{
    public class Useractivityrepository : Iactivity
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Useractivityrepository(IHttpContextAccessor httpContextAccessor)
        {
            // _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InsertEmailHistoryAsync(EmailHistory request)
        {
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string connectionString = SqlHelper.GetConnectiondetails(appType1);
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("InsertEmailHistory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SenderEmail", request.SenderEmail);
                    command.Parameters.AddWithValue("@RecipientEmail", request.RecipientEmail);
                    command.Parameters.AddWithValue("@Subject", request.Subject);
                    command.Parameters.AddWithValue("@Body", request.Body);
                    command.Parameters.AddWithValue("@SentDateTime", request.SentDateTime);
                    command.Parameters.AddWithValue("@ArchiveFlag", request.ArchiveFlag);
                    command.Parameters.AddWithValue("@User_Guid", request.UserGuid);
                    command.Parameters.AddWithValue("@CreateAt", request.CreateAt);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<IEnumerable<EmailHistoryResponse>> GetEmailHistoryByUserGuidAsync(string userGuid, DateTime? sentDateTime = null, string subject = null)
        {
            var emailHistories = new List<EmailHistoryResponse>();
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetEmailHistoryByUserGuid", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@User_Guid", userGuid);
                    command.Parameters.AddWithValue("@SentDateTime", (object)sentDateTime ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Subject", (object)subject ?? DBNull.Value);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var emailHistory = new EmailHistoryResponse
                            {
                                EmailId = reader.GetInt32("EmailId"),
                                EmailGUId = reader.GetGuid(reader.GetOrdinal("Email_Guid")).ToString(),
                                SenderEmail = reader.GetString(reader.GetOrdinal("SenderEmail")),
                                RecipientEmail = reader.GetString(reader.GetOrdinal("RecipientEmail")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Body = reader.GetString(reader.GetOrdinal("Body")),
                                SentDateTime = reader.GetDateTime(reader.GetOrdinal("SentDateTime")),
                                ArchiveFlag = reader.GetBoolean(reader.GetOrdinal("ArchiveFlag")),
                                UserGuid = reader.GetString(reader.GetOrdinal("User_Guid")),
                                CreateAt = reader.GetDateTime(reader.GetOrdinal("CreateAt"))
                            };

                            emailHistories.Add(emailHistory);
                        }
                    }
                }
            }
            return emailHistories;
        }

        public async Task<IEnumerable<UserActivityData>> GetUserActivitiesAsync(string userGuid = null, string userActivity = null, DateTime? createdOn = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var userActivities = new List<UserActivityData>();
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetUserActivities", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@User_Guid", (object)userGuid ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UserActivity", (object)userActivity ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedOn", (object)createdOn ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FromDate", (object)fromDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ToDate", (object)toDate ?? DBNull.Value);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var UserActivityData = new UserActivityData
                            {
                                UserActivityId= reader.GetInt32("UserActivityId"),
                                UserGuid = reader.GetString(reader.GetOrdinal("User_Guid")),
                                UserActivity = reader.GetString(reader.GetOrdinal("UserActivity")),
                                CreatedOn = reader.GetDateTime(reader.GetOrdinal("Createdon")),
                                Ipaddress = reader.GetString(reader.GetOrdinal("ip_address")),
                                macaddress = reader.GetString(reader.GetOrdinal("mac_address")),
                                //CreatedOn = reader.GetDateTime(reader.GetOrdinal("Createdon"))
                            };

                            userActivities.Add(UserActivityData);
                        }
                    }
                }
            }

            return userActivities;
        }


        public async Task InsertUserActivityAsync(UserActivityData userActivityData)
        {
            var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
            string _connectionString = SqlHelper.GetConnectiondetails(appType1);
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("InsertUserActivity", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@User_Guid", userActivityData.UserGuid);
                    command.Parameters.AddWithValue("@UserActivity", userActivityData.UserActivity);
                    command.Parameters.AddWithValue("@Description", (object)userActivityData.description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Createdon", userActivityData.CreatedOn);
                    command.Parameters.AddWithValue("@ip_address", userActivityData.Ipaddress);
                    command.Parameters.AddWithValue("@mac_address", userActivityData.macaddress);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
