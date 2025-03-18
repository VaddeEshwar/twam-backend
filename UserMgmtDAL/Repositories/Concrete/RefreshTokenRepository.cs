using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;

public class RefreshTokenRepository : IrefreshToken
{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task InsertRefreshTokenAsync(int userId, string refreshToken)
    {
        var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
        string connectionString = SqlHelper.GetConnectiondetails(appType1);
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("StoreRefreshToken", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@UserId", userId));
                command.Parameters.Add(new SqlParameter("@RefreshToken", refreshToken));
                command.Parameters.Add(new SqlParameter("@ExpiryDate", DateTime.UtcNow.AddDays(30))); // Set refresh token expiry

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<string> GetUserIdFromRefreshTokenAsync(string refreshToken)
    {
        var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
        string connectionString = SqlHelper.GetConnectiondetails(appType1);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("GetUserIdFromRefreshToken", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter for refresh token
                command.Parameters.Add(new SqlParameter("@RefreshToken", SqlDbType.NVarChar, 200) { Value = refreshToken });

                // Output parameter for User_Guid
                SqlParameter userIdParam = new SqlParameter("@User_Guid", SqlDbType.VarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(userIdParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                // Check the output parameter value
                string userGuid = userIdParam.Value as string;

                return string.IsNullOrEmpty(userGuid) ? "0" : userGuid; // Return "0" if User_Guid is null or empty
            }
        }
    }


    public async Task InsertRefreshTokenAsync(RefreshToken objrefreshToken)
    {
        var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
        string connectionString = SqlHelper.GetConnectiondetails(appType1);
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("StoreRefreshToken", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@User_Guid", objrefreshToken.userGUID);
                    command.Parameters.AddWithValue("@RefreshToken", objrefreshToken.refreshToken);
                    command.Parameters.AddWithValue("@ExpiryDate", objrefreshToken.expiryDate);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> GetStoredRefreshTokenAsync(string userGuid)
    {

        var appType1 = _httpContextAccessor.HttpContext?.Request.Headers["X-App-Type"].ToString();
        string connectionString = SqlHelper.GetConnectiondetails(appType1);
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("GetStoredRefreshToken", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@User_Guid", SqlDbType.VarChar, 255) { Value = userGuid });

                var refreshTokenParam = new SqlParameter("@RefreshToken", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(refreshTokenParam);

                await command.ExecuteNonQueryAsync();

                return refreshTokenParam.Value as string;
            }
        }
    }
}
