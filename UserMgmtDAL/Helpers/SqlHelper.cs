using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace UserMgmtDAL.Helpers
{
    public class SqlHelper
    {
        public static string constr;
        SqlConnection conn;
        public static IConfiguration _configuration; static string databaseConfigFile = "appsettings.json";

        public SqlHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static string GetConnectiondetails(string appType)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(databaseConfigFile);
            _configuration = builder.Build();
            string type = _configuration["Mailsettings:smtp"];
            string strPatshala = _configuration["AppdbtypeSettings:Patshala"];
            string strTwam = _configuration["AppdbtypeSettings:Twam"];
            string strOurDallas = _configuration["AppdbtypeSettings:OurDallas"];
            string connectionString;
            if (appType == strPatshala)
            {
                connectionString = _configuration.GetConnectionString("PatashalaSQLConnection");
            }
            else if (appType == strTwam)
            {
                connectionString = _configuration.GetConnectionString("PatashalaSQLConnection1");
            }
            else if (appType == strOurDallas)
            {
                connectionString = _configuration.GetConnectionString("PatashalaSQLConnection1");
            }
            else
            {
                throw new InvalidOperationException("Unknown AppType in request.");
            }

            return connectionString;
        }


        public static string GetAppType(IHttpContextAccessor httpContextAccessor)
        {
            // Access HttpContext and retrieve the header value safely
            var appType = httpContextAccessor?.HttpContext?.Request.Headers["X-App-Type"].ToString();
            return appType ?? string.Empty;  // Return empty string if null
        }





    }
}
