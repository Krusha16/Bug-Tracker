using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class MembershipHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        static RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>
            (new RoleStore<IdentityRole>(db));
        static UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>
            (new UserStore<ApplicationUser>(db));

        public static List<string> GetAllRoles()
        {
            var roles = roleManager.Roles.Select(r => r.Name).ToList();
            return roles;
        }

        public static bool CheckIfUserIsInRole(string userId, string role)
        {
            var result = userManager.IsInRole(userId, role);
            return result;
        }

        public static bool AddUserToRole(string userId, string role)
        {
            if (CheckIfUserIsInRole(userId, role))
                return false;
            else
            {
                userManager.AddToRole(userId, role);
                return true;
            }
        }

        public static bool RemoveUserFromRole(string userId, string role)
        {
            if (CheckIfUserIsInRole(userId, role))
                return false;
            else
            {
                userManager.RemoveFromRole(userId, role);
                return true;
            }
        }

        public static void AddRole(string roleName)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole { Name = roleName });
            }
        }
        public static void RemoveRole(string roleName)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Delete(new IdentityRole { Name = roleName });
            }
        }

        public static List<string> GetAllRolesOfUser(string UserId)
        {
            return userManager.GetRoles(UserId).ToList();
        }
    }
}