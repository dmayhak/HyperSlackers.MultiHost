using MultiHostDemo.Entities;
using MultiHostDemo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiHostDemo.Models
{
    public abstract class ModelBase
    {
        private Guid currentUserId = Guid.Empty;
        private AppUser currentUser = null;
        private readonly Guid currentHostId = Guid.Empty;

        protected internal static ApplicationDbContext Repo { get { return Current.Context; } }

        protected internal Guid CurrentUserId
        {
            get
            {
                if (currentUserId == Guid.Empty)
                {
                    currentUserId = Current.UserId;
                }

                return currentUserId;
            }
        }

        protected internal AppUser CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    currentUser = Current.User;
                    currentUserId = currentUser.Id;
                }

                return currentUser;
            }
        }
    }
}