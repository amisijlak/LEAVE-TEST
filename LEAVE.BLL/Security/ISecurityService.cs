using LEAVE.DAL.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEAVE.BLL.Security
{
    public interface ISecurityService
    {
        Task<bool> CreateUserAsync(ApplicationUser user, List<UserRoleModel> Roles, string loginURL);
        Task<bool> UpdateUserAsync(ApplicationUser user, List<UserRoleModel> Roles);
        Task<bool> ChangePasswordAsync(ChangePasswordModel model);
        Task<bool> AdminResetPasswordAsync(ResetPasswordModel model);
        Task<bool> ActivateUserAsync(LockoutModel model);
        Task<bool> DeactivateUserAsync(LockoutModel model);
        Task<object> DeleteUsersAsync(List<string> ids);

        Task UnlockSuperAdmin();

        Task<bool> SaveRoleAsync(ISecurityRole model);
        bool DeleteRole(string Id, out string ErrorMessage);

        void UpdatePermissions(List<PermissionType> protectedActionAttributes);
    }

    public enum UserImportMode
    {
        Create,
        Create_And_Update_Using_UserName_As_The_Key,
        Create_And_Update_Using_Email_As_The_Key,
    }

    public class UserEditTemplateModel
    {
        public string code { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string othername { get; set; }
        public string email { get; set; }
        public string phonenumber { get; set; }
        public int usertype { get; set; }
        public string roles { get; set; }
    }

    public class UserRoleModel
    {
        [Required(ErrorMessage = "Required!")]
        public string RoleId { get; set; }
        public string Name { get; set; }
    }

    

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Required!")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Required!")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Required!")]
        public string ConfirmPassword { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class LockoutModel
    {
        public string Id { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ResetPasswordModel
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
        public string ErrorMessage { get; set; }
    }
}
