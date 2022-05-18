using LEAVE.BLL.Data;
using LEAVE.BLL.Helpers;
using LEAVE.DAL;
using LEAVE.DAL.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEAVE.BLL.Security
{
    public class SecurityService : ISecurityService
    {
        private readonly IDbRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<SecurityRole> roleManager;
        private readonly ISessionService sessionService;
        private readonly IEmailSender emailSender;

        public SecurityService(IDbRepository _repository, UserManager<ApplicationUser> _userManager, RoleManager<SecurityRole> _roleManager
            , ISessionService _sessionService, IEmailSender emailSender)
        {
            this.repository = _repository;
            this.roleManager = _roleManager;
            this.userManager = _userManager;
            this.sessionService = _sessionService;
            this.emailSender = emailSender;
        }

        private static Random random = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void _validateUser(ApplicationUser user, List<UserRoleModel> Roles)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("Email Address Required!");
            }
            else if (Roles.GroupBy(r => r.RoleId).Any(r => r.Count() > 1))
            {
                throw new Exception("Roles MUST be unique!");
            }
            else if (repository.Set<ApplicationUser>().Any(r => r.UserName == user.UserName && r.Id != user.Id))
            {
                throw new Exception("Another User with the same USERNAME already exists!");
            }
            else if (repository.Set<ApplicationUser>().Any(r => r.Code == user.Code && r.Id != user.Id))
            {
                throw new Exception("Another User with the same CODE already exists!");
            }
            else if (repository.Set<ApplicationUser>().Any(r => r.Email == user.Email && r.Id != user.Id))
            {
                throw new Exception("Another User with the same Email already exists!");
            }
            else if (!sessionService.IsInSuperRole())//prevent Super Role Assignment
            {
                var superRole = repository.GetSuperRole();

                if (Roles.Any(r => r.RoleId == superRole.Id)) throw new Exception($"Role '{CONSTANTS.SUPER_ROLE}' Not Assignable!");
            }

            user.FormatCode();
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, List<UserRoleModel> Roles, string loginURL)
        {
            try
            {
                //validation
                _validateUser(user, Roles);

                var roleIds = Roles.Select(r => r.RoleId).ToList();
                var roleNames = repository.Set<SecurityRole>().Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();

                var password = RandomString(8);

                user.Id = Guid.NewGuid().ToString();

                await _handleTaskAsync(userManager.CreateAsync(user, password), () =>
                {
                    return _handleTaskAsync(userManager.AddToRolesAsync(user, roleNames));
                });

                string errorMessage = null;

                if (!emailSender.SendEmail(new[] { user.Email }, $"LEAVE Account Created", $@"
Dear {user.FirstName},<br/><br/>

Your account to the LEAVE has been created.<br/>
Please use the username <b>{user.UserName}</b> and password <b>{password}</b> to Login <a href='{loginURL}' target='_blank'>here.</a><br/><br/>

This is a system generated email, please do not reply.

", out errorMessage))
                {
                    user.ErrorMessage = "Error sending email: " + errorMessage;
                }

                return true;
            }
            catch (Exception e)
            {
                user.ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user, List<UserRoleModel> Roles)
        {
            try
            {
                //validation
                _validateUser(user, Roles);

                var dbUser = repository.Set<ApplicationUser>().Find(user.Id);
                if (dbUser == null) throw new Exception("User Not Found!");

                if ((dbUser.IsSuperUser() && !sessionService.IsSuperAdmin()))
                {
                    throw new Exception("You are NOT authorized to Update this account!");
                }
                else if (!sessionService.IsInSuperRole() && repository.IsUserInSuperRole(dbUser.Id))
                {
                    throw new Exception("You are NOT authorized to Update this account!");
                }

                //update user fields
                if (dbUser.IsSuperUser()) dbUser.UserName = user.UserName;
                dbUser.FirstName = user.FirstName;
                dbUser.LastName = user.LastName;
                dbUser.OtherName = user.OtherName;
                dbUser.Email = user.Email;
                dbUser.PhoneNumber = user.PhoneNumber;
                dbUser.Code = user.Code;
                dbUser.UserType = user.UserType;
                dbUser.InstitutionId = user.InstitutionId;
                dbUser.BranchId = user.BranchId;

                repository.SaveChanges();

                //update roles
                if (!dbUser.IsSuperUser())
                {

                    var updatedRoleIds = Roles.Select(r => r.RoleId).ToList();
                    var updatedRoleNames = repository.Set<SecurityRole>().Where(r => updatedRoleIds.Contains(r.Id)).Select(r => r.Name).ToList();
                    await _updateUserRolesAsync(dbUser, updatedRoleNames);
                }

                return true;
            }
            catch (Exception e)
            {
                user.ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }
        public async Task<object> DeleteUsersAsync(List<string> ids)
        {
            string errorMessage = null;
            var deletionCount = 0;
            var failed = 0;
            var notDeletable = 0;
            bool success = false;

            try
            {
                var currentUserId = sessionService.GetUserId();

                foreach (var id in ids)
                {
                    var dbUser = repository.GetAccessibleUsers(sessionService).Where(r => r.Id == id).SingleOrDefault();

                    if (dbUser != null && dbUser.Id != currentUserId && !dbUser.IsSuperUser())
                    {
                        var result = await userManager.DeleteAsync(dbUser);
                        if (result.Succeeded)
                        {
                            deletionCount++;
                        }
                        else
                        {
                            failed++;
                        }
                    }
                    else
                    {
                        notDeletable++;
                    }
                }

                success = true;
            }
            catch (Exception e)
            {
                errorMessage = e.ExtractInnerExceptionMessage();
            }

            return new
            {
                success,
                errorMessage,
                summary = $"Deleted {deletionCount.FormatForDisplay()} users, {failed.FormatForDisplay()} failed and {notDeletable.FormatForDisplay()} are not deletable."
            };
        }

        private async Task _updateUserRolesAsync(ApplicationUser dbUser, List<string> updatedRoleNames)
        {
            //remove previous roles
            var result = await userManager.GetRolesAsync(dbUser);
            await userManager.RemoveFromRolesAsync(dbUser, result);

            //add new roles
            await _handleTaskAsync(userManager.AddToRolesAsync(dbUser, updatedRoleNames));
        }

        private async Task _handleTaskAsync(Task<IdentityResult> task, Func<Task> callbackFtn = null)
        {
            var result = await task;

            if (result.Succeeded)
            {
                if (callbackFtn != null) await callbackFtn();
            }
            else
            {
                throw new Exception(string.Join(",", result.Errors.Select(r => $"{r.Code}: {r.Description}")));
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordModel model)
        {
            try
            {
                //validation
                if (string.Compare(model.NewPassword, model.ConfirmPassword) != 0)
                {
                    throw new Exception("The Two New Passwords MUST match!");
                }

                var UserId = sessionService.GetUserId();

                var dbUser = repository.Set<ApplicationUser>().Find(UserId);
                if(dbUser == null) throw new Exception("User Not Found!");

                await _handleTaskAsync(userManager.ChangePasswordAsync(dbUser, model.CurrentPassword, model.NewPassword));

                return true;
            }
            catch (Exception e)
            {
                model.ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }

        public async Task<bool> AdminResetPasswordAsync(ResetPasswordModel model)
        {
            try
            {
                var dbUser = repository.Set<ApplicationUser>().Find(model.UserId);
                if (dbUser == null) throw new Exception("User Not Found!");

                if (!dbUser.IsAccessible(repository, sessionService)) throw new Exception("User NOT Accessible!");

                if (dbUser.IsSuperUser())
                {
                    throw new Exception("You CAN'T Reset the Super User's password!");
                }

                string resetToken = await userManager.GeneratePasswordResetTokenAsync(dbUser);

                await _handleTaskAsync(userManager.ResetPasswordAsync(dbUser, resetToken, model.NewPassword));

                return true;
            }
            catch (Exception e)
            {
                model.ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }

        public async Task<bool> SaveRoleAsync(ISecurityRole model)
        {
            try
            {
                #region Validations

                if (repository.Set<SecurityRole>().Any(r => r.Id != model.Id && r.Name == model.Name))//role name validation
                {
                    throw new Exception("Another Role with the same name already exists!");
                }

                #endregion

                Action<string> updateRolePermissions = (roleId) =>
                {
                    var PermissionList = model.GetPermissions();

                    var dbSet = repository.Set<RolePermission>();

                    var newPermissions = PermissionList.Where(r => r.Enabled).ToList();
                    var previousPermissions = dbSet.Where(r => r.RoleId == roleId).ToList();

                    //Save New Permissions

                    foreach (var permission in newPermissions)
                    {
                        var dbPermission = previousPermissions.Where(r => r.PermissionId == permission.PermissionId).SingleOrDefault();

                        if (dbPermission == null)//New Permission
                        {
                            dbSet.Add(new RolePermission
                            {
                                RoleId = roleId,
                                PermissionId = permission.PermissionId
                            });
                        }
                        else//Existing Permission
                        {
                            previousPermissions.Remove(dbPermission);
                        }
                    }

                    // Delete Removed Role Permissions i.e. Remaining in List
                    dbSet.RemoveRange(previousPermissions);

                    repository.SaveChanges();
                };

                if (model.IsNewRecord)//create
                {
                    var roleId = await _createRole(model.Name, updateRolePermissions);
                }
                else//update
                {
                    var dbRole = repository.Set<SecurityRole>().Where(r => r.Id == model.Id).SingleOrDefault();
                    if (dbRole == null) throw new Exception("Role Not Found!");

                    if (!dbRole.IsAccessible(repository, sessionService)) throw new Exception("Role Not Accessible To You!");

                    if (dbRole.IsSuperRole())
                    {
                        throw new Exception("You CAN'T edit the Super Role!");
                    }

                    var beforeXML = dbRole.GenerateLogXML("Before Update");

                    dbRole.Name = model.Name;
                    await _updateRole(dbRole, updateRolePermissions);
                }

                return true;
            }
            catch (Exception e)
            {
                model.ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }

        public virtual async Task<string> _createRole(string roleName, Action<string> successCallbackFtn)
        {
            var role = new SecurityRole(roleName) { Id = Guid.NewGuid().ToString() };
            await _handleTaskAsync(roleManager.CreateAsync(role), () => Task.Run(() => successCallbackFtn(role.Id)));

            return role.Id;
        }

        public virtual async Task _updateRole(SecurityRole dbRole, Action<string> successCallbackFtn)
        {
            await _handleTaskAsync(roleManager.UpdateAsync(dbRole), () => Task.Run(() => successCallbackFtn(dbRole.Id)));
        }

        public async Task<bool> ActivateUserAsync(LockoutModel model)
        {
            return await _manageUserLockoutAsync(model, DateTime.Today.AddDays(-1));
        }

        public async Task<bool> DeactivateUserAsync(LockoutModel model)
        {
            return await _manageUserLockoutAsync(model, DateTime.Today.AddYears(50));
        }

        private async Task<bool> _manageUserLockoutAsync(LockoutModel model, DateTime LockoutEndDate)
        {
            try
            {
                var dbUser = repository.Set<ApplicationUser>().Find(model.Id);
                if (dbUser == null) throw new Exception("User Not Found!");

                if (!dbUser.IsAccessible(repository, sessionService)) throw new Exception("User NOT Accessible!");

                if (dbUser.IsSuperUser())
                {
                    throw new Exception("You CAN'T update the Super User!");
                }
                else if (dbUser.IsCurrentUser(sessionService))
                {
                    throw new Exception("You CAN'T manipulate your account!");
                }

                await _handleTaskAsync(userManager.SetLockoutEndDateAsync(dbUser, LockoutEndDate));

                return true;
            }
            catch (Exception e)
            {
                model.ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }

        public async Task UnlockSuperAdmin()
        {
            try
            {
                var superUser = repository.Set<ApplicationUser>().Where(r => r.UserName == CONSTANTS.SUPER_USER).SingleOrDefault();
                if (superUser == null) return;

                var lockedOut = await userManager.IsLockedOutAsync(superUser);

                if (lockedOut)
                {
                    await _handleTaskAsync(userManager.SetLockoutEndDateAsync(superUser, DateTime.Today.AddDays(-1)));
                }
            }
            catch (Exception e)
            {
            }
        }

        public bool DeleteRole(string Id, out string ErrorMessage)
        {
            ErrorMessage = null;

            try
            {
                repository.ExecuteInNewTransaction(() =>
                {
                    var dbRole = repository.Set<SecurityRole>().Find(Id);
                    if (dbRole == null) throw new Exception("Role Not Found!");

                    if (dbRole.IsSuperRole())
                    {
                        throw new Exception("You CAN'T delete the Super Role!");
                    }

                    string RoleName = dbRole.Name;

                    repository.Set<SecurityRole>().Remove(dbRole);
                    repository.SaveChanges();
                });

                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }

        public void UpdatePermissions(List<PermissionType> protectedActionAttributes)
        {
            try
            {
                repository.ExecuteInNewTransaction(() =>
                {
                    var existingPermissions = repository.Set<PermissionType>().ToList();

                    //Loop through discovered protected actions
                    foreach (var discoveredItem in protectedActionAttributes.ToList())
                    {
                        var existingItem = existingPermissions.Where(r => r.Module == discoveredItem.Module && r.SubModule == discoveredItem.SubModule
                          && r.SystemAction == discoveredItem.SystemAction).SingleOrDefault();

                        if (existingItem != null)//Already Exists
                        {
                            existingPermissions.Remove(existingItem);
                        }
                        else//New Action
                        {
                            repository.Set<PermissionType>().Add(discoveredItem);
                        }
                    }

                    repository.SaveChanges();

                    //Delete Removed actions
                    foreach (var item in existingPermissions)
                    {
                        repository.Set<PermissionType>().Remove(item);
                    }

                    repository.SaveChanges();
                });
            }
            catch (Exception e)
            {
            }
        }
    }
}
