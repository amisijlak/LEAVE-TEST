using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.DAL.Security
{
    public class PermissionType:_BaseNumericModel
    {
        #region Fields

        public SecurityModule Module { get; set; }
        public SecuritySubModule SubModule { get; set; }
        public SecuritySystemAction SystemAction { get; set; }

        #endregion

        #region Navigation Properties

        public virtual List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        #endregion
    }
}
