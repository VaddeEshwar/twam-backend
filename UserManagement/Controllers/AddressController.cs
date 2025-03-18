using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;
using UserMgmtDAL.Repositories.Concrete;
using System;
using UserMgmtDAL.Constants;
using Microsoft.AspNetCore.Cors;
using System.Threading.Tasks;


namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("corsDevelopment")]
    public class AddressController : ControllerBase
    {
        public readonly Iaddress _addressrepository;

        public AddressController(Iaddress addressService)
        {
            _addressrepository = addressService;
        }

        //[HttpPost("addAddress")]
        //public async Task<IActionResult> AddAddress([FromBody] Address addressDto)
        //{
        //    if (addressDto == null)
        //    {
        //        return BadRequest("Invalid address data.");
        //    }
        //    Addressreturnval addressval = new Addressreturnval();

        //    addressval = await _addressrepository.InsertAddressAsync(addressDto); 
        //    if (addressval.sucessval == true)
        //        return Ok(addressval.strMsg);
        //    else
        //        return StatusCode(500, addressval.strMsg);

        //}

        //[HttpPost("UserAddressMap")]
        //public async Task<IActionResult> MapUserAddress(userAddress objaddress)
        //{
        //    if (objaddress == null || objaddress.userId <= 0 || objaddress.addressId <= 0)
        //    {
        //        return BadRequest("Invalid request");
        //    }

        //    var userAddressId = await _addressrepository.InsertuserAddress(objaddress);

        //    return Ok(new { UserAddressId = userAddressId });
        //}
    }
}
