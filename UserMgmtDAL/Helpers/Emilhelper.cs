using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using UserMgmtDAL.Models;


namespace UserMgmtDAL.Helpers
{
    public class Emilhelper
    {
        public readonly IConfiguration _configuration;
        //string strhost = Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

        public Emilhelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendVerificationEmail(string email, string verificationLink, string verificationGuid)
        {
            //var host1= _configuration["Mailsettings:smtp"]; 
            var host = _configuration["Mailsettings:smtp"];
            int port = Convert.ToInt32(_configuration["Mailsettings:port"]);
            string strfromemail = _configuration["Mailsettings:Fromemail"];
            string strpwd = _configuration["Mailsettings:password"];
            EmailHistory emailHistory;
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(strfromemail),
                    Subject = "Email Verification",
                    Body = $"Please verify your email by clicking <a href=\"{verificationLink}\">here</a>.",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                using (var smtpClient = new SmtpClient(host, port))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(strfromemail, strpwd);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                    emailHistory = new EmailHistory();
                    emailHistory.SenderEmail = strfromemail;
                    emailHistory.RecipientEmail = email;
                    emailHistory.Subject = mailMessage.Subject;
                    emailHistory.Body = mailMessage.Body;
                    emailHistory.SentDateTime = DateTime.UtcNow;
                    emailHistory.CreateAt = DateTime.UtcNow;
                    emailHistory.UserGuid = verificationGuid;
                    emailHistory.ArchiveFlag = true;
                    InsertEmailHistoryAsync(emailHistory);

                }
            }
            catch (SmtpException ex)
            {
                // Log the exception (ex) here for further investigation
                throw new ApplicationException("An error occurred while sending the email.", ex);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for further investigation
                throw new ApplicationException("An unexpected error occurred while sending the email.", ex);
            }

        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                //var host1= _configuration["Mailsettings:smtp"]; 
                //var host = _configuration["Mailsettings:smtp"];
                //int port = Convert.ToInt32(_configuration["Mailsettings:port"]);
                //string strfromemail = _configuration["Mailsettings:Fromemail"];
                //string strpwd = _configuration["Mailsettings:password"];
                EmailHistory emailHistory;
                //var mailMessage = new MailMessage
                //{
                //    From = new MailAddress(strfromemail),
                //    Subject = subject,
                //    Body = body,
                //    IsBodyHtml = true
                //};

                //mailMessage.To.Add(toEmail);

                //using (var smtpClient = new SmtpClient(host, port))
                //{
                //    smtpClient.Credentials = new NetworkCredential(strfromemail, strpwd);
                //    smtpClient.EnableSsl = true;

                //    await smtpClient.SendMailAsync(mailMessage);
                //}
                using (MailMessage mailMessage = new MailMessage())
                {
                    var host = _configuration["Mailsettings:smtp"];
                    int port = Convert.ToInt32(_configuration["Mailsettings:port"]);
                    string strfromemail = _configuration["Mailsettings:Fromemail"];
                    string strpwd = _configuration["Mailsettings:password"];
                    mailMessage.From = new MailAddress(strfromemail);
                    mailMessage.To.Add(toEmail);

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    // Attach files if available


                    using (SmtpClient smtpClient = new SmtpClient(host, port))
                    {
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.Credentials = new NetworkCredential(strfromemail, strpwd);
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Send(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task InsertEmailHistoryAsync(EmailHistory request)
        {
            string strdbconnection = _configuration["ConnectionStrings:PatashalaSQLConnection"];
            // string connectionString = SqlHelper.GetConnectiondetails(appType1);
            using (var connection = new SqlConnection(strdbconnection))
            {
                using (var command = new SqlCommand("InsertEmailHistory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SenderEmail", request.SenderEmail);
                    command.Parameters.AddWithValue("@RecipientEmail", request.RecipientEmail);
                    command.Parameters.AddWithValue("@Subject", request.Subject);
                    command.Parameters.AddWithValue("@Body", request.Body);
                    command.Parameters.AddWithValue("@SentDateTime", request.SentDateTime);
                    command.Parameters.AddWithValue("@ArchiveFlag", request.ArchiveFlag);
                    command.Parameters.AddWithValue("@User_Guid", request.UserGuid);
                    command.Parameters.AddWithValue("@CreateAt", request.CreateAt);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
