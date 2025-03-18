using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using static UserMgmtDAL.Models.UserActivity;

namespace UserMgmtDAL.Helpers
{
    public class Useractivityhistory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Useractivityhistory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
                    command.Parameters.AddWithValue("@Createdon", System.DateTime.UtcNow);
                    command.Parameters.AddWithValue("@ip_address", GetLocalIPAddress());
                    command.Parameters.AddWithValue("@mac_address", GetMACAddress());

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public static string GetLocalIPAddress()
        {
            string localIP = string.Empty;

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            localIP = unicastAddress.Address.ToString();
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(localIP))
                {
                    break;
                }
            }

            return localIP;
        }
        public static string GetMACAddress()
        {
            string macAddress = string.Empty;

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                {
                    macAddress = string.Join(":", networkInterface.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                    break;
                }
            }

            return macAddress;
        }

    }

}

