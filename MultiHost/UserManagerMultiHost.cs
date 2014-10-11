using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    public class UserManagerMultiHost<TUser, TKey, THostKey> : UserManager<TUser, TKey>
        where TUser : IdentityUserMultiHost<TKey, THostKey>, new()
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        public virtual THostKey HostId { get; set; }
        private bool disposed = false;

        public UserManagerMultiHost(UserStoreMultiHost<TUser, TKey, THostKey> store)
            : base(store)
        {
            // allow duplicate emails and funky chars
            this.UserValidator = new UserValidator<TUser, TKey>(this) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = false };
        }

        public UserManagerMultiHost(UserStoreMultiHost<TUser, TKey, THostKey> store, THostKey hostId)
            : this(store)
        {
            this.HostId = hostId;
        }

        public bool IsInRole(TKey userId, TKey roleId)
        {
            var user = Store.FindByIdAsync(userId).Result;
            if (user != null)
            {
                return user.Roles.Any(r => r.RoleId.Equals(roleId));
            }

            return false;
        }

        public void RemoveFromRole(TKey userId, TKey roleId)
        {
            var user = Store.FindByIdAsync(userId).Result;
            if (user != null)
            {
                user.Roles.Remove(user.Roles.Where(r => r.RoleId.Equals(roleId)).Single());
            }
        }

        public void CreateUser(TUser user, string password)
        {
            base.CreateAsync(user);

            base.AddPasswordAsync(user.Id, password);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // TODO: cache references? if so, release them here

                    this.disposed = true;
                }
            }

            base.Dispose(disposing);
        }
    }

    public class UserManagerMultiHostString : UserManagerMultiHost<IdentityUserMultiHost<string, string>, string, string>
    {
        public UserManagerMultiHostString(UserStoreMultiHost<IdentityUserMultiHost<string, string>, string, string> store)
            : this(store, null)
        {
        }

        public UserManagerMultiHostString(UserStoreMultiHost<IdentityUserMultiHost<string, string>, string, string> store, string hostId)
            : base(store, hostId)
        {
        }
    }

    public class UserManagerMultiHostGuid : UserManagerMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid>
    {
        public UserManagerMultiHostGuid(UserStoreMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid> store)
            : this(store, Guid.Empty)
        {
        }

        public UserManagerMultiHostGuid(UserStoreMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid> store, Guid hostId)
            : base(store, hostId)
        {
        }
    }

    public class UserManagerMultiHostInt : UserManagerMultiHost<IdentityUserMultiHost<int, int>, int, int>
    {
        public UserManagerMultiHostInt(UserStoreMultiHost<IdentityUserMultiHost<int, int>, int, int> store)
            : this(store, 0)
        {
        }

        public UserManagerMultiHostInt(UserStoreMultiHost<IdentityUserMultiHost<int, int>, int, int> store, int hostId)
            : base(store, hostId)
        {
        }
    }

    public class UserManagerMultiHostLong : UserManagerMultiHost<IdentityUserMultiHost<long, long>, long, long>
    {
        public UserManagerMultiHostLong(UserStoreMultiHost<IdentityUserMultiHost<long, long>, long, long> store)
            : base(store)
        {
        }

        public UserManagerMultiHostLong(UserStoreMultiHost<IdentityUserMultiHost<long, long>, long, long> store, long hostId)
            : base(store, hostId)
        {
        }
    }
}
