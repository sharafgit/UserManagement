using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement.Contants;

namespace UserManagement.Seeds
{
    public static class DefaulteUsers
    {
        public static async Task SeedBasicUserAsync(UserManager<IdentityUser> userManager)
        {
            var defaultUser = new IdentityUser()
            {
                UserName = "basicuser@domain.com",
                Email = "basicuser@domain.com",
                EmailConfirmed = true
            };

            var user = userManager.FindByEmailAsync(defaultUser.Email);

            if (user==null)
            {
                await userManager.CreateAsync(defaultUser, "P@ssaword123");
                await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
            }
        }




        public static async Task SeedSuperAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManger)
        {
            var defaultUser = new IdentityUser()
            {
                UserName = "superadmin@domain.com",
                Email = "superadmin@domain.com",
                EmailConfirmed = true
            };

            var user = userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "P@ssaword123");
                await userManager.AddToRolesAsync(defaultUser,new List<string> { Roles.Basic.ToString(),Roles.Admin.ToString(),Roles.SuperAdmin.ToString() });

            }

            await roleManger.SeedClaimsForSuperUser();

        }

        private static async Task SeedClaimsForSuperUser(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(adminRole, "Products");
        }

        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allCliaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsList(module);

            foreach (var Permission in allPermissions)
            {
                if (allCliaims.Any(c => c.Type == "Permission" && c.Value == Permission))
                    await roleManager.AddClaimAsync(role, new Claim("Permission", Permission));

            }
        }



    }
}
