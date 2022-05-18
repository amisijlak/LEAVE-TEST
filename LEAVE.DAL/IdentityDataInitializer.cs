using LEAVE.DAL.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEAVE.DAL
{
    public static class IdentityDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<SecurityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync(CONSTANTS.SUPER_USER).Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.Id = Guid.NewGuid().ToString();
                user.UserName = CONSTANTS.SUPER_USER;
                user.FirstName = "Super";
                user.LastName = "Admin";
                user.Email = "interswitch@gmail.com";
                user.Code = "ADMIN";


                IdentityResult result = userManager.CreateAsync(user, "administrator").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, CONSTANTS.SUPER_ROLE).Wait();
                }
                else
                {
                    throw new Exception($"Default User Creation Error(s): {string.Join(",", result.Errors.Select(r => $"{r.Code}: {r.Description}"))}");
                }
            }
        }

        private static void SeedRoles(RoleManager<SecurityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(CONSTANTS.SUPER_ROLE).Result)
            {
                SecurityRole role = new SecurityRole(CONSTANTS.SUPER_ROLE);
                role.Id = Guid.NewGuid().ToString();
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Default Role Creation Error(s): {string.Join(",", roleResult.Errors.Select(r => $"{r.Code}: {r.Description}"))}");
                }
            }
        }
    }
}
