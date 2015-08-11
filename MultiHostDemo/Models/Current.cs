using MultiHostDemo.Entities;
using MultiHostDemo.ExtensionMethods;
using MultiHostDemo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MultiHostDemo.Models
{
    internal static class Current
    {
        public static readonly Guid SystemUserId = new Guid("bf784c72-c224-47d0-9ca6-2f719d46adbc");

        private static ApplicationDbContext winowsDbContext = null; // only used for windows apps, not for web

        internal static Guid GetUserOrSystemId()
        {
            return UserId != Guid.Empty ? UserId : SystemUserId;
        }

        internal static Guid UserId
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context != null)
                {
                    if (context.User != null)
                    {
                        return context.User.UserId();
                    }
                }

                return Guid.Empty;
            }
        }

        internal static AppUser User
        {
            get
            {
                Guid id = UserId;

                if (id == Guid.Empty)
                {
                    return null;
                }

                return Context.Users.Find(id);
            }
        }

        internal static string UserName()
        {
            if (HttpContext.Current != null)
            {
                // web application
                if (UserId == Guid.Empty || UserId == SystemUserId || User == null)
                {
                    return "<system>";
                }
                else
                {
                    return User.FullName;
                }
            }
            else
            {
                // windows application
                // TODO:
                //throw new NotImplementedException();
                return "<system>";
            }
        }

        internal static ApplicationDbContext Context
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // web app
                    ApplicationDbContext context = (ApplicationDbContext)HttpContext.Current.Items["CurrentContext"];

                    if (context == null)
                    {
                        // should not get here
                        // TODO: watch for threading/locking issues if we do get here
                        context = new ApplicationDbContext();
                        HttpContext.Current.Items.Add("CurrentContext", context);
                    }

                    return context;
                }
                else
                {
                    // win app

                    return winowsDbContext ?? (winowsDbContext = new ApplicationDbContext());
                }
            }
        }
    }
}
