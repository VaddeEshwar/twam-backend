using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;
using System.Collections.Generic;
using UserMgmtDAL.Repositories.Concrete;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("corsDevelopment")]
    public class CommonController : ControllerBase
    {
        private readonly IRolerepository _roleRepository;
        private readonly ICommonRepository _commonRepository;
        public CommonController(IRolerepository reportService, ICommonRepository commonRepository) {

            _roleRepository= reportService;
            _commonRepository = commonRepository;
        }
        
        [HttpGet]
        [Route("GetRoles")]
        //[Authorize]
        public async Task<IActionResult> GetRoles()
        {
            List<RoleDto> roles = await _roleRepository.GetRolesAsync();
            return Ok(roles);
        }

        [HttpGet("GetCountries")]
        public async Task<ActionResult<List<Country>>> GetCountries(string? searchKeyword)
        {
            try
            {
                List<Country> countries = await _commonRepository.GetCountries(searchKeyword);
                return Ok(countries); // Return 200 OK with the list of countries
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 in case of any exceptions
            }
        }

        [HttpPost("GetStates")]
        public async Task<ActionResult<List<State>>> GetStates(int countryId, string? searchKeyword)
        {
            try
            {
                List<State> states = await _commonRepository.GetStates(countryId, searchKeyword);
                return Ok(states); // Return 200 OK with the list of states
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 in case of any exceptions
            }
        }

        [HttpGet("GetCities")]
        public async Task<ActionResult<List<City>>> GetCities(int stateId, string? searchKeyword)
        {
            try
            {
                List<City> cities = await _commonRepository.GetCity(stateId, searchKeyword);
                return Ok(cities); // Return 200 OK with the list of cities
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 in case of any exceptions
            }
        }

    }
}
