using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace UserMgmtDAL.Repositories.Concrete
{
    public class DataRepositoryBase
    {
        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;

        public DataRepositoryBase(ILogger logger, IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));

        }
    }
}