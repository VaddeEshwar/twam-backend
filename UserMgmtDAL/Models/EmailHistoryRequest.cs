using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Models
{
    public class EmailHistory
    {
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentDateTime { get; set; }
        public bool ArchiveFlag { get; set; }
        public string UserGuid { get; set; }
        public DateTime CreateAt { get; set; }
    }

    public class EmailHistoryResponse
    {
        public int EmailId { get; set; }
        public string EmailGUId { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentDateTime { get; set; }
        public bool ArchiveFlag { get; set; }
        public string UserGuid { get; set; }
        public DateTime CreateAt { get; set; }
    }

    public class EmailHistoryRequest
    {      
    
       
        public string? Subject { get; set; }
      
        public DateTime? SentDateTime { get; set; }
        
        public string UserGuid { get; set; }
    
    }
    public class UserActivityInsert
    {
        public string UserGuid { get; set; }
        public string UserActivity { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
    }

}
