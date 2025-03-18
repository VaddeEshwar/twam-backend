using UserMgmtDAL.Repositories.Abstract;
using UserMgmtDAL.Repositories.Concrete;
using System.Reflection.Metadata;

namespace UserManagement.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddServices(this IServiceCollection Services, IConfiguration configuration)
        {
        
            //Services.AddTransient<DataRepositoryBase>();
            Services.AddHttpClient();
            // Accessing the configuration for JwtToken settings


            return Services;

        }
    }
}
