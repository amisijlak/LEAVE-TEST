using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LEAVE.DAL.Security
{
    public class SecurityRole : IdentityRole<string>, INamedModel, IPrimaryKeyEnabled<string>
    {
        public virtual List<RolePermission> Permissions { get; set; } = new List<RolePermission>();

        public SecurityRole() : base() { }
        public SecurityRole(string roleName) : base(roleName) { }
    }

    public class RolePermission : _BaseNumericModel
    {
        #region Fields

        [ForeignKey("Permission")]
        public long PermissionId { get; set; }
        [ForeignKey("Role"), Required]
        public string RoleId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual PermissionType Permission { get; set; }
        public virtual SecurityRole Role { get; set; }

        #endregion
    }
}
