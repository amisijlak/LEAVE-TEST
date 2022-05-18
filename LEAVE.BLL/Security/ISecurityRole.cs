using LEAVE.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.BLL.Security
{
    /// <summary>
    /// A Security Role Model
    /// </summary>
    public interface ISecurityRole
    {
        string Id { get; set; }
        string Name { get; set; }
        bool IsNewRecord { get; set; }
        string ErrorMessage { get; set; }
        List<ISecurityPermission> GetPermissions();
    }

    public interface ISecurityPermission
    {
        long PermissionId { get; set; }
        bool Enabled { get; set; }
        SecurityModule Module { get; set; }
        SecuritySubModule SubModule { get; set; }
        SecuritySystemAction SystemAction { get; set; }
    }
}
