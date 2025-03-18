using JWTAuthenticationManager;
using JWTAuthenticationManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserMgmtDAL.Constants;
using UserMgmtDAL.Helpers;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;
using UserMgmtDAL.Repositories.Concrete;

//using Flyurdreamcommands.Helpers;
using static UserMgmtDAL.Models.UserActivity;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("corsDevelopment")]
    public class UserController : ControllerBase
    {
        public readonly IUserRepository _authService;
        public readonly Iaddress _addressrepository;
        public readonly IConfiguration _configuration;
        public readonly TokenService _tokenService;
        private readonly IrefreshToken _refreshtokenService;
        private readonly ITokenhandler _tokenhandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserRepository authService, TokenService TokenService, IConfiguration configuration, ITokenhandler tokenhandler, IrefreshToken irefreshToken, IHttpContextAccessor httpContextAccessor)//,TokenService TokenService, Iaddress addresaservice , IUserRepository authService
        {
            _authService = authService;
            _tokenService = TokenService;
            _configuration = configuration;
            _refreshtokenService = irefreshToken;
            _tokenhandler = tokenhandler;
            _httpContextAccessor = httpContextAccessor;
            //  _addressrepository = addresaservice;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            // IUserRepository _authService;

            registeroutval objregisteroutval = new registeroutval();
            Emilhelper objemilhelper = new Emilhelper(_configuration);
            try
            {
                var user = await _authService.RegisterUserAsync(request);
                if (user.userId != 0)
                {
                    //string verificationLink = _configuration["Emailsettings:verificationlink"] + "api/user/verify?guid=" + user.verificationGuid;
                    string verificationLink = _configuration["Emailsettings:verificationlink"] +_configuration["Emailsettings:apiUrl"] + user.verificationGuid;
                    await objemilhelper.SendVerificationEmail(request.Email, verificationLink, user.verificationGuid.ToString());
                }
                return Ok(new
                {
                    Message = Const.success_checkyouremail,
                    UserId = user.userId,
                });
            }
            catch (ApplicationException ex)
            {
                // Log the exception (ex) here for further investigation
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for further investigation
                return StatusCode(500, new { Message = ex.Message });
                //return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
        }

        //[AllowAnonymous]
        //[HttpPost("Loginn")]
        //public ActionResult Loginn(LoginRequest userLogin)
        //{
        //    var user = Authenticate(userLogin);
        //    if (user != null)
        //    {
        //        var token = GenerateToken(user);
        //        return Ok(token);
        //    }

        //    return NotFound("user not found");
        //}
        private string GenerateToken(LoginRequest user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,"raki2830")
                //new Claim(ClaimTypes.NameIdentifier,user.Username)
                //,new Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        private LoginRequest Authenticate(LoginRequest userLogin)
        {
            LoginRequest loginRequest = new LoginRequest();
            loginRequest.Username = "raki2830";
            loginRequest.Password = "hello";
            var currentUser = loginRequest;
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { Message = "Invalid request payload." });
            }

            try
            {
                var user = await _authService.GetUserByUsernameAsync(request);

                if (user == null)
                {



                    return Unauthorized(new { Message = Const.Incorrect_password });

                }

                if (user.emalverified == 0)//(!await _authService.IsEmailVerifiedAsync(user.Email))
                {
                    return BadRequest(new { Message = "Email not verified. Please verify your email before logging in." });
                }

                if (SecurityHelper.VerifyPassword(request.Password, user.PasswordHash.ToString(), user.salt))
                {
                    // Generate JWT or session token here

                    //string strtoken = _tokenService.GenerateAccessToken(user.userGuid);
                    string struserId = user.userGuid.ToString();
                    AuthenticationResponse objauthresp = _tokenhandler.GenerateAccessToken(struserId);
                    string strrefreshtoken = _tokenhandler.GenerateRefreshToken();
                    var token = new RefreshToken
                    {
                        refreshToken = strrefreshtoken,
                        userGUID = user.userGuid,
                        expiryDate = _tokenhandler.GetExpiryDate()
                    };
                    _refreshtokenService.InsertRefreshTokenAsync(token);
                  
                    return Ok(new { Message = Const.Login_successful, UserId = user.userId, Email = user.Email, token = objauthresp.JwtToken, refreshtoken = strrefreshtoken });
                }
                else
                {
                    return Unauthorized(new { Message = Const.Incorrect_password });
                }
                if(user.userGuid !="")
                {
                    UserActivityData userActivityData= new UserActivityData();
                    userActivityData.UserGuid = user.userGuid;
                    userActivityData.UserActivity = "Login";
                    userActivityData.description = Const.Login_successful;
                    Useractivityhistory useractivityhistory = new Useractivityhistory(_httpContextAccessor);
                    useractivityhistory.InsertUserActivityAsync(userActivityData);
                  
                }
            }
            catch (ApplicationException ex)
            {
                // Log the exception (ex) here for further investigation
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for further investigation
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }


        [HttpGet("verify/{guid}")]
        public async Task<IActionResult> VerifyEmail(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return BadRequest(new { Message = Const.Invalidverificationlink });
            }

            try
            {
                var result = await _authService.VerifyEmailAsync(guid);

                if (result.userId != 0)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "An error occurred while verifying your email.");
                }

            }
            catch (ApplicationException ex)
            {
                // Log the exception (ex) here for further investigation
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for further investigation
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("updateuserdetails")]
        public async Task<IActionResult> UpdateUser(UpdateuserRequest request)
        {
            if (request == null || request.userGuid == string.Empty)
            {
                return BadRequest("Invalid request");
            }

            var status = await _authService.UpdateUserAsync(request);
           
          

            if (status == "User not found or no changes made")
            {
                return NotFound(status);
            }

            return Ok(status);
        }


        [HttpGet]
        [Route("Getping")]
        //[Authorize]
        public List<string> Getping(DateTime dateTime)
        {
            // Retrieve the client's IP address from the HTTP request
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Return the IP address in a list of strings
            return new List<string> { ipAddress };
        }

        [HttpGet]
        [Authorize]
        [Route("GetDbsuccess")]
        public async Task<string> GetDbsuccess()
        {
            var response = "";
            try
            {
                response = await _authService.GetDatabsucces();


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }
        //[HttpGet]
        //[Route("testemail")]
        //public async Task<ActionResult<bool>> testemail()
        //{
        //    Emilhelper objemilhelper = new Emilhelper(_configuration);
        //    try
        //    { var email = "vamshi1@mailinator.com";
        //        var isValid = true; var response = "";

        //        if (isValid)
        //        {
        //            await objemilhelper.SendEmailAsync(email,"test","test");
        //            return Ok(true); // Return true if the OTP is valid
        //        }
        //        else
        //        {
        //            return Ok(false); // Return false if the OTP is invalid
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if necessary
        //        // ...

        //        // Return a generic error message to the client
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("OtpValidation")]
        public async Task<ActionResult<bool>> OtpValidation(string email, string passcode)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(passcode))
            {
                return BadRequest("Email and passcode are required.");
            }

            try
            {
                bool isValid = await _authService.ValidateOTP(email, passcode);
                if (isValid)
                {
                    return Ok(true); // Return true if the OTP is valid
                }
                else
                {
                    return Ok(false); // Return false if the OTP is invalid
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                // ...

                // Return a generic error message to the client
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
