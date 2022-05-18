using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.BLL.Helpers
{
    public interface IEmailSender
    {
        bool SendEmail(string[] Addresses, string Subject, string Body, out string ErrorMessage);
    }
}
