using LEAVE.BLL.Data;
using LEAVE.BLL.Security;
using LEAVE.DAL.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LEAVE.DAL.Models.Security
{
    public class RoleContainerModel : ISecurityRole
    {
        public bool IsNewRecord { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public List<SecurityRole> Roles { get; set; } = new List<SecurityRole>();
        public string ErrorMessage { get; set; }

        public List<RolePermissionViewModel> Permissions { get; set; } = new List<RolePermissionViewModel>();

        public RoleContainerModel()
        {
        }

        public void LoadRoles(IDbRepository repository)
        {
           Roles = repository.Set<SecurityRole>().ToList();
        }
        public List<ISecurityPermission> GetPermissions()
        {
            return Permissions.Select(r => (ISecurityPermission)r).ToList();
        }

        public static RoleContainerModel GetRoleDetails(IDbRepository repository, string Id)
        {
            var dbRole = repository.Set<SecurityRole>().Find(Id);
            bool IsNew = false;

            if (dbRole == null)//Initialize New Record
            {
                IsNew = true;
                dbRole = new SecurityRole();
            }

            //Build Header
            RoleContainerModel model = new RoleContainerModel
            {
                Id = Id,
                Name = dbRole.Name,
                IsNewRecord = IsNew
            };

            //Fetch existing role permissions
            var dbRolePermissions = repository.Set<RolePermission>().Where(r => r.RoleId == Id).ToList();


            //Map Global Permissions to Role Permissions
            var accessibleModules = ProtectActionAttribute.AccessibleModules;

            foreach (var permission in repository.Set<PermissionType>().Where(r => accessibleModules.Contains(r.Module)).ToList())
            {
                model.Permissions.Add(new RolePermissionViewModel
                {
                    SystemAction = permission.SystemAction,
                    Module = permission.Module,
                    PermissionId = permission.Id,
                    SubModule = permission.SubModule,
                    Enabled = dbRolePermissions.Any(r => r.PermissionId == permission.Id)
                });
            }

            return model;
        }
    }

    public class RolePermissionViewModel : ISecurityPermission
    {
        public long PermissionId { get; set; }
        public bool Enabled { get; set; }
        public SecurityModule Module { get; set; }
        public SecuritySubModule SubModule { get; set; }
        public SecuritySystemAction SystemAction { get; set; }
    }
}
