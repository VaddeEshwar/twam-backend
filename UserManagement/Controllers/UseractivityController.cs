using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
using UserMgmtDAL.Constants;
using UserMgmtDAL.Models;
using UserMgmtDAL.Repositories.Abstract;
using static UserMgmtDAL.Models.UserActivity;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UseractivityController : ControllerBase
    {
        public readonly Iactivity _actService;
        public UseractivityController(Iactivity iactivity)
        {
            _actService = iactivity;
        }

        //[HttpPost("insertEmailHistory")]
        //public async Task<IActionResult> InsertEmailHistory([FromBody] EmailHistory request)
        //{
        //    if (request == null)
        //    {
        //        return BadRequest("Invalid request");
        //    }

        //    await _actService.InsertEmailHistoryAsync(request);


        //    return Ok(Const.Email_historySuccessfully);
        //}


        [HttpPost("GetEmailHistorybyUserId")]
        public async Task<IActionResult> GetEmailHistory(EmailHistoryRequest request)
        {
            if (string.IsNullOrEmpty(request.UserGuid))
            {
                return BadRequest(Const.Noemail_history);
            }

            var emailHistories = await _actService.GetEmailHistoryByUserGuidAsync(request.UserGuid, request.SentDateTime, request.Subject);

            if (emailHistories == null )
            {
                return NotFound(Const.Noemail_history);
            }

            return Ok(emailHistories);
        }

        [HttpPost("getUserActivities")]
        public async Task<IActionResult> GetUserActivities(UserActivityinputparms objUserActivityinputparms)
        {
            var userActivities = await _actService.GetUserActivitiesAsync(objUserActivityinputparms.UserGuid, objUserActivityinputparms.UserActivity , objUserActivityinputparms.CreatedOn, objUserActivityinputparms.Fromdate, objUserActivityinputparms.Todate);

            if (userActivities == null )
            {
                return NotFound(Const.Nouser_activities);
            }

            return Ok(userActivities);
        }

       
        //public async Task<IActionResult> InsertUserActivity([FromBody] UserActivityData request)
        //{
        //    if (request == null)
        //    {
        //        return BadRequest("Invalid request");
        //    }

        //    await _actService.InsertUserActivityAsync(request
        //        //request.UserGuid,
        //        //request.UserActivity,
        //        //request.description,
        //        //request.CreatedOn,
        //        //request.Ipaddress,
        //        //request.macaddress
        //    );

        //    return Ok(Const.Useractivity_Successfully);
        //}


    }
}




