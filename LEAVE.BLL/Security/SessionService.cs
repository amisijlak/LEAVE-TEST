using LEAVE.BLL.Data;
using LEAVE.DAL;
using LEAVE.DAL.Entities;
using LEAVE.DAL.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEAVE.BLL.Security
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbRepository _repository;
        private List<string> SecurityRoleIds = new List<string>();
        private List<SessionServicePermission> Permissions = new List<SessionServicePermission>();

        private const string INTITUTION_COOKIE = "IntitutionCookie";

        //user data
        private ApplicationUser User;
        private bool _IsInSuperRole;
        private SecurityModule? _module;
        private SecuritySubModule? _submodule;
        private SecuritySystemAction? _systemaction;
        private string _IPAddress;

        private Institution currentDistributor;

        public SecurityModule? GetCurrentActionSecurityModule() => _module;
        public SecuritySubModule? GetCurrentActionSecuritySubModule() => _submodule;
        public SecuritySystemAction? GetCurrentActionSecuritySystemAction() => _systemaction;

        public string GetIPAddress() => _IPAddress;

        public SessionService(IHttpContextAccessor _httpContextAccessor, IDbRepository _repository)
        {
            this._httpContextAccessor = _httpContextAccessor;
            this._repository = _repository;

            if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                var UserName = _httpContextAccessor.HttpContext.User.Identity.Name;

                User = _repository.Set<ApplicationUser>().Where(r => r.UserName == UserName).SingleOrDefault();

                InitializeUser();
            }
        }

        private void InitializeUser()
        {
            SecurityRoleIds = _repository.Set<IdentityUserRole<string>>().Where(r => r.UserId == User.Id).Select(r => r.RoleId).Distinct().ToList();

            _IsInSuperRole = _repository.Set<SecurityRole>().Any(r => SecurityRoleIds.Contains(r.Id) && r.Name == CONSTANTS.SUPER_ROLE);

            Permissions = (from a in _repository.Set<RolePermission>()
                           where SecurityRoleIds.Contains(a.RoleId)
                           select new SessionServicePermission
                           {
                               SystemAction = a.Permission.SystemAction,
                               Module = a.Permission.Module,
                               SubModule = a.Permission.SubModule
                           }).Distinct().ToList();

            _IPAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public void InitializeForApiUser(ApplicationUser _user)
        {
            if (_user == null) return;

            User = _user;

            InitializeUser();
        }

        public List<string> GetSecurityRoleIds() => SecurityRoleIds;

        public void DestroyCookies()
        {
            foreach (var cookieKey in new[] { INTITUTION_COOKIE })
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieKey);
            }
        }

        public virtual long? _GetCookieValue(string CookieKey)
        {
            try
            {
                var programId = Convert.ToInt32(_httpContextAccessor.HttpContext.Request.Cookies[CookieKey]);

                if (programId > 0) return programId;
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public void _SetCookieValue(string CookieKey, long Value)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieKey, Value.ToString());
        }

        public string GetUserId()
        {
            return User?.Id;
        }

        public string GetUserName()
        {
            return User?.UserName;
        }

        public bool IsSuperAdmin()
        {
            return User?.IsSuperUser() ?? false;
        }

        public bool IsInSuperRole()
        {
            return _IsInSuperRole;
        }

        public ApplicationUser GetUser()
        {
            return User;
        }

        public bool HasAccessToPermission(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action, bool IgnoreSystemAdminPriviledge = false)
        {
            if (_IsInSuperRole && !IgnoreSystemAdminPriviledge)
            {
                return true;
            }
            else if (Permissions.Any(r => r.Module == Module && r.SubModule == SubModule && r.SystemAction == Action))
            {
                return true;
            }

            return false;
        }

        public void SetProtectedAction(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action)
        {
            this._module = Module;
            this._submodule = SubModule;
            this._systemaction = Action;
        }

        private class MobilePermissionModel
        {
            public string Id { get; set; }
            public long? SubModule { get; set; }
            public long? SystemAction { get; set; }
        }

        public class SessionServicePermission : ISecurityPermission
        {
            public long PermissionId { get; set; }
            public bool Enabled { get; set; }
            public SecurityModule Module { get; set; }
            public SecuritySubModule SubModule { get; set; }
            public SecuritySystemAction SystemAction { get; set; }
        }
    }
}
