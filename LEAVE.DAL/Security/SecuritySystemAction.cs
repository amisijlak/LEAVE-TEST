using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LEAVE.DAL
{
    public enum SecuritySystemAction
    {
        //generic actions under 1000
        [Description("Create and Edit")]
        CreateAndEdit = 1,
        Delete = 2,
        [Description("View List")]
        ViewList = 3,
        Review = 10,

        //security specific actions 1000-9999
        Login = 1004,
        LogOff = 1005,
        [Description("Change Password")]
        ChangePassword = 1006,
        [Description("Reset Password")]
        ResetPassword = 1007,
        Activate = 1008,
        Deactivate = 1009,

        //distributor specific actions 10000-19999
        [Description("Restart Day")]
        RestartDay = 10000,
    }
}
