using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace LEAVE.BLL.Helpers
{
    public class EmailSender : IEmailSender
    {
        private string SmtpHost;
        private string SenderEmailAddress;
        private string SenderPassword;
        private bool Enabled;
        private int SmtpPort;
        private bool UseSsl;

        public EmailSender(bool Enabled, string SmtpHost, int SmtpPort, bool UseSsl, string SenderEmailAddress, string SenderPassword)
        {
            this.Enabled = Enabled;
            this.SmtpHost = SmtpHost;
            this.SmtpPort = SmtpPort;
            this.UseSsl = UseSsl;
            this.SenderEmailAddress = SenderEmailAddress;
            this.SenderPassword = SenderPassword;
        }

        public bool SendEmail(string[] Addresses, string Subject, string Body, out string ErrorMessage)
        {
            ErrorMessage = null;

            if (!Enabled) return true;

            try
            {
                using (SmtpClient SmtpServer = new SmtpClient(SmtpHost))
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(SenderEmailAddress);

                        foreach (var address in Addresses)
                        {
                            if (!string.IsNullOrEmpty(address))
                            {
                                mail.To.Add(address.Trim());
                            }
                        }

                        if (!mail.To.Any()) throw new Exception("No Recipients Found!");

                        mail.Subject = Subject;
                        mail.Body = Body;
                        mail.IsBodyHtml = true;

                        SmtpServer.Port = SmtpPort;
                        SmtpServer.UseDefaultCredentials = false;
                        SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(SenderEmailAddress, SenderPassword);
                        SmtpServer.EnableSsl = UseSsl;
                        SmtpServer.Timeout = 60000;//1 minute
                        SmtpServer.Send(mail);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }
    }
}
