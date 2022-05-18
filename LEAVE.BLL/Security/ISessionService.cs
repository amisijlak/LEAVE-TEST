using LEAVE.DAL;
using LEAVE.DAL.Entities;
using LEAVE.DAL.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.BLL.Security
{
    public interface ISessionService
    {
        string GetUserId();
        string GetUserName();
        bool IsSuperAdmin();
        bool IsInSuperRole();
        ApplicationUser GetUser();
        List<string> GetSecurityRoleIds();

        string GetIPAddress();

        long? _GetCookieValue(string CookieKey);
        void DestroyCookies();

        void SetProtectedAction(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action);
        bool HasAccessToPermission(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action, bool IgnoreSystemAdminPriviledge = false);

        SecurityModule? GetCurrentActionSecurityModule();
        SecuritySubModule? GetCurrentActionSecuritySubModule();
        SecuritySystemAction? GetCurrentActionSecuritySystemAction();

        void InitializeForApiUser(ApplicationUser _user);
    }
}
