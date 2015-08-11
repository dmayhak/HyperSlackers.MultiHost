using Microsoft.AspNet.Identity;
using MultiHostDemo.Entities;
using MultiHostDemo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MultiHostDemo.ExtensionMethods
{
    public static class IPrincipalExtensions
    {
        private static ApplicationDbContext Repo { get { return new ApplicationDbContext(); } }

        public static bool IsInRole(this IPrincipal user, RoleType role)
        {
            //Contract.Requires<ArgumentNullException>(user != null, "user");

            return user.IsInRole(role.ToString());
        }

        public static Guid UserId(this IPrincipal user)
        {
            //Contract.Requires<ArgumentNullException>(user != null, "user");

            Guid id = Guid.Empty;
            if (Guid.TryParse(user.Identity.GetUserId(), out id))
            {
                return id;
            }

            return Guid.Empty;
        }

        internal static AppUser GetAppUser(this IPrincipal user)
        {
            Guid id = user.UserId();

            if (id == Guid.Empty)
            {
                return null;
            }

            return Repo.Users.Find(id); // could fail... check for nulls.
        }

        public static string Email(this IPrincipal user)
        {
            //Contract.Requires<ArgumentNullException>(user != null, "user");

            AppUser appUser = user.GetAppUser();

            if (appUser == null)
            {
                return string.Empty;
            }

            return appUser.Email;
        }

        public static string FirstName(this IPrincipal user)
        {
            //Contract.Requires<ArgumentNullException>(user != null, "user");

            AppUser appUser = user.GetAppUser();

            if (appUser == null)
            {
                return string.Empty;
            }

            return appUser.FirstName;
        }

        public static string LastName(this IPrincipal user)
        {
            //Contract.Requires<ArgumentNullException>(user != null, "user");

            AppUser appUser = user.GetAppUser();

            if (appUser == null)
            {
                return string.Empty;
            }

            return appUser.LastName;
        }
    }
}

