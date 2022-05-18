using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LEAVE.DAL
{
    public enum SecurityModule
    {
        [Description("Mobile Application")]
        MobileApplication = 1,
        Security = 2,
        Settings = 3,
        Distributor = 4,
        Surveys = 5,
        BI=6
    }
}
