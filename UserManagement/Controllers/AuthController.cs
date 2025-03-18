using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using UserMgmtDAL.Constants;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;
using JWTAuthenticationManager;
using System.Threading.Tasks;
using System;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IrefreshToken _refreshtokenService;
        private readonly ITokenhandler _tohenhandler;
        RefreshToken objrefrehtoken = new RefreshToken();
      //  Tokenhandler tokenhandler = new Tokenhandler(TokenOptions);
        public AuthController( IrefreshToken refreshtokenService,ITokenhandler tokenhandler)
        {
          // _tokenService = tokenService;
            _refreshtokenService = refreshtokenService;
            _tohenhandler = tokenhandler;
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] RefreshToken refreshtokenrequest)
        {
            try
            {
                if (refreshtokenrequest is null || string.IsNullOrEmpty(refreshtokenrequest.refreshToken))
                    return BadRequest("Invalid request");

                //string userId = await _refreshtokenService.GetUserIdFromRefreshTokenAsync(request.RefreshToken);
                var principal = _tohenhandler.GetPrincipalFromExpiredToken(refreshtokenrequest.refreshToken);
                if (principal == null)
                    return Unauthorized();

                // Generate new access token
                var newAccessToken = _tohenhandler.GenerateAccessToken(principal.Identity.Name);
                string strRefreshtoken = _tohenhandler.GenerateRefreshToken();

                var expiryDate = _tohenhandler.GetExpiryDate();
                objrefrehtoken.refreshToken = strRefreshtoken;
                objrefrehtoken.expiryDate = expiryDate;
                objrefrehtoken.userGUID = refreshtokenrequest.userGUID;
                _refreshtokenService.InsertRefreshTokenAsync(objrefrehtoken);

                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = strRefreshtoken// _tokenService.GenerateRefreshToken()
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> GetRefreshToken(string userGuid)
        {
            try
            {
                var token = await _refreshtokenService.GetStoredRefreshTokenAsync(userGuid);
                if (token != null)
                {
                    return Ok(new { RefreshToken = token });
                }
                else
                {
                    return NotFound(Const.Refreshtoken_Notfound);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetUserIdFromRefreshTokenAsync")]
        public async Task<IActionResult> GetUserIdFromRefreshTokenAsync(string refreshToken)
        {
            try
            {
                string userId = await _refreshtokenService.GetUserIdFromRefreshTokenAsync(refreshToken);

                if (userId == "0")
                {
                    return Unauthorized(); // Token is invalid or expired
                }

                // Proceed with the valid userId
                return Ok(new { UserId = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
