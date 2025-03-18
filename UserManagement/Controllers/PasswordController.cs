using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlTypes;
using System.Net;
using System.Threading.Tasks;
using UserMgmtDAL.Constants;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;
using static UserMgmtDAL.Models.UserActivity;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        public readonly Ipassword _pwdService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IConfiguration _configuration;

        public PasswordController(Ipassword pwdService, IHttpContextAccessor httpContextAccessor,IConfiguration configuration1)//,TokenService TokenService, Iaddress addresaservice , IUserRepository authService
        {
            _pwdService = pwdService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration1;

        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var (status, userGuid) = await _pwdService.ChangePasswordAsync(request.Email, request.NewPassword, request.Salt);

            if (userGuid == Guid.Empty)
            {

                return StatusCode(400, new { Status = status });

            }
            else
            {
                UserActivityData userActivityData = new UserActivityData();
                userActivityData.UserGuid = userGuid.ToString();
                userActivityData.UserActivity = "OTP Genration";
                userActivityData.description = Const.OTP_Updated;
                Useractivityhistory useractivityhistory = new Useractivityhistory(_httpContextAccessor);
                useractivityhistory.InsertUserActivityAsync(userActivityData);

                return Ok(new { UserGUID = userGuid });
            }
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequest request)
        {
            Emilhelper objemilhelper = new Emilhelper(_configuration);
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Invalid request");
            }

            var (passcode, expiry, result, firstName, strGuId) = await _pwdService.GenerateOtpAsync(request.Email);

            if (result == 1)
            {
                return Ok(new
                {
                    Passcode = passcode,
                    Expiry = expiry,
                    FirstName = firstName,
                    GuId = strGuId
                });
                string subject = "Your OTP Code";
                string body = $"Your OTP code is: <strong>{passcode}</strong>";

                await objemilhelper.SendEmailAsync(request.Email, subject, body);
                //await objemilhelper.SendVerificationEmail(request.Email, verificationLink, user.verificationGuid.ToString());
                UserActivityData userActivityData = new UserActivityData();
                userActivityData.UserGuid = strGuId;
                userActivityData.UserActivity = "OTP Genration";
                userActivityData.description = Const.OTP_Updated;
                Useractivityhistory useractivityhistory = new Useractivityhistory(_httpContextAccessor);
                useractivityhistory.InsertUserActivityAsync(userActivityData);

            }
            else
            {
                return NotFound(new { Result = result });
            }
        }
    }
}
