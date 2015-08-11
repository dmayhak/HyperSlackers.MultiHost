using Microsoft.AspNet.Identity;
using MultiHostDemo.Entities;
using MultiHostDemo.Models;
using MultiHostDemo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MultiHostDemo.ExtensionMethods
{
    public static class IIdentityExtensions
    {
        public static AppUser GetUser(this IIdentity identity)
        {
            var user = Current.User;

            return user;
        }

        // TODO: not needed since we can get user above? or are these just nice to have?
        public static string GetEmail(this IIdentity identity)
        {
            var user = Current.User;

            if (user != null)
            {
                return user.Email;
            }

            return null;
        }

        public static string GetFirstName(this IIdentity identity)
        {
            Guid userId = Guid.Empty;
            if (Guid.TryParse(identity.GetUserId(), out userId))
            {
                AppUser user = (new ApplicationDbContext()).Users.Find(userId);

                if (user != null)
                {
                    return user.FirstName;
                }
            }

            return null;
        }
    }
}
