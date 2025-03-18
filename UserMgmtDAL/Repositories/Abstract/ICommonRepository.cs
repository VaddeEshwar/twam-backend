using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgmtDAL.Models;

namespace UserMgmtDAL.Repositories.Abstract
{
    public interface ICommonRepository
    {
        Task<List<Country>> GetCountries(string? searchKeyword);
        Task<List<State>> GetStates(int CountryId, string? SearchKeyword);
        Task<List<City>> GetCity(int StateId, string? SearchKeyword);
    }
}
