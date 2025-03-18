using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgmtDAL.Models;
using static UserMgmtDAL.Models.UserActivity;

namespace UserMgmtDAL.Repositories.Abstract
{
    public interface Iactivity
    {
        Task InsertEmailHistoryAsync(EmailHistory request);
        Task<IEnumerable<EmailHistoryResponse>> GetEmailHistoryByUserGuidAsync(string userGuid, DateTime? sentDateTime = null, string subject = null);
        Task<IEnumerable<UserActivityData>> GetUserActivitiesAsync(string userGuid = null, string userActivity = null, DateTime? createdOn = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task InsertUserActivityAsync(UserActivityData userActivityData);

    }


}

