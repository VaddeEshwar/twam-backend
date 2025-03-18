using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;

namespace UserMgmtDAL.Repositories.Concrete
{
    public class CommonRepository : DataRepositoryBase, ICommonRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        
        public CommonRepository(IHttpContextAccessor httpContextAccessor,IConfiguration config, ILogger<CommonRepository> logger) : base(logger, config)
        {
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Country>> GetCountries(string? searchKeyword)
        {
            var appType1 = SqlHelper.GetAppType(_httpContextAccessor);
            string connectionString = SqlHelper.GetConnectiondetails(appType1);
            List<Country> countries = new List<Country>();
            try
            {
                await using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    await using (SqlCommand command = new SqlCommand("SearchCountry", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (string.IsNullOrEmpty(searchKeyword))
                        {
                            command.Parameters.Add(new SqlParameter("@SearchKeyword", DBNull.Value));
                        }
                        else
                        {
                            command.Parameters.Add(new SqlParameter("@SearchKeyword", searchKeyword));
                        }
                        command.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output });
                        await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Country country = new Country();
                                country.CountryID = reader.GetInt32(reader.GetOrdinal("Country_ID"));
                                country.CountryName = reader.IsDBNull(reader.GetOrdinal("Country_Name")) ? null : reader.GetString(reader.GetOrdinal("Country_Name"));
                                country.CountryCode = reader.IsDBNull(reader.GetOrdinal("Country_Code")) ? null : reader.GetString(reader.GetOrdinal("Country_Code"));
                                country.Dial = reader.IsDBNull(reader.GetOrdinal("dial")) ? 0 : reader.GetInt32(reader.GetOrdinal("dial"));
                                country.Currency_Name = reader.IsDBNull(reader.GetOrdinal("currency_name")) ? null : reader.GetString(reader.GetOrdinal("currency_name"));
                                country.Currency = reader.IsDBNull(reader.GetOrdinal("currency")) ? null : reader.GetString(reader.GetOrdinal("currency"));

                                countries.Add(country);
                            }
                        }
                    }

                     connection.Close();
                }
            }
            catch (Exception ex) { _logger.LogError(ex.Message); throw; }

            return countries;
        }

        public async Task<List<State>> GetStates(int CountryId, string? SearchKeyword)
        {
            List<State> states = new List<State>();
            try
            {
                var appType1 = SqlHelper.GetAppType(_httpContextAccessor);
                string connectionString = SqlHelper.GetConnectiondetails(appType1);
              await  using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    await using (SqlCommand command = new SqlCommand("SearchStateByCountry", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@SearchKeyword", (object)SearchKeyword ?? DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@CountryID", CountryId));
                        command.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output });


                        await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                State state = new State();
                                state.CountryID = Convert.ToInt32(reader["Country_ID"]);
                                state.StateName = reader["State_Name"].ToString();
                                state.StateID = Convert.ToInt32(reader["State_ID"]);
                                states.Add(state);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            return states;
        }

        public async Task<List<City>> GetCity(int StateId, string? SearchKeyword)
        {
            List<City> cities = new List<City>();
            try
            {
                var appType1 = SqlHelper.GetAppType(_httpContextAccessor);
                string connectionString = SqlHelper.GetConnectiondetails(appType1);
                await using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    await using (SqlCommand command = new SqlCommand("SearchCityByState", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@SearchKeyword", (object)SearchKeyword ?? DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@StateID", StateId));
                        command.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output });

                        await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                City city = new City();
                                city.StateID = Convert.ToInt32(reader["State_ID"]);
                                city.CityID = Convert.ToInt32(reader["City_ID"]);
                                city.CityName = reader["City_Name"].ToString();
                                cities.Add(city);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

            return cities;
        }


    }

}
