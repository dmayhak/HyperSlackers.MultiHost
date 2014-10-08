using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    public class UserManagerMultiHost<TUser, TKey, THostKey> : UserManager<TUser, TKey>
        where TUser : IdentityUserMultiHost<TKey, THostKey, IdentityUserLoginMultiHost<TKey, THostKey>, IdentityUserRoleMultiHost<TKey>, IdentityUserClaimMultiHost<TKey>>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        public virtual THostKey HostId { get; set; }

        public UserManagerMultiHost(UserStoreMultiHost<TUser, IdentityRoleMultiHost<TKey, THostKey, IdentityUserRoleMultiHost<TKey>>, TKey, THostKey, IdentityUserLoginMultiHost<TKey, THostKey>, IdentityUserRoleMultiHost<TKey>, IdentityUserClaimMultiHost<TKey>> store)
            : base(store)
        {

        }

        public UserManagerMultiHost(UserStoreMultiHost<TUser, IdentityRoleMultiHost<TKey, THostKey, IdentityUserRoleMultiHost<TKey>>, TKey, THostKey, IdentityUserLoginMultiHost<TKey, THostKey>, IdentityUserRoleMultiHost<TKey>, IdentityUserClaimMultiHost<TKey>> store, THostKey hostId)
            : this(store)
        {
            this.HostId = hostId;
        }
    }

    public class UserManagerMultiHostString : UserManagerMultiHost<IdentityUserMultiHost<string, string, IdentityUserLoginMultiHost<string, string>, IdentityUserRoleMultiHost<string>, IdentityUserClaimMultiHost<string>>, string, string>
    {
        public UserManagerMultiHostString(UserStoreMultiHostString store)
            : this(store, null)
        {

        }

        public UserManagerMultiHostString(UserStoreMultiHostString store, string hostId)
            : base(store, hostId)
        {

        }
    }

    public class UserManagerMultiHostGuid : UserManagerMultiHost<IdentityUserMultiHost<Guid, Guid, IdentityUserLoginMultiHost<Guid, Guid>, IdentityUserRoleMultiHost<Guid>, IdentityUserClaimMultiHost<Guid>>, Guid, Guid>
    {
        public UserManagerMultiHostGuid(UserStoreMultiHostGuid store)
            : this(store, Guid.Empty)
        {

        }

        public UserManagerMultiHostGuid(UserStoreMultiHostGuid store, Guid hostId)
            : base(store, hostId)
        {
            // allow duplicate emails and funky chars
            this.UserValidator = new UserValidator<IdentityUserMultiHost<Guid, Guid, IdentityUserLoginMultiHost<Guid, Guid>, IdentityUserRoleMultiHost<Guid>, IdentityUserClaimMultiHost<Guid>>, Guid>(this) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = false };
        }
    }

    public class UserManagerMultiHostInt : UserManagerMultiHost<IdentityUserMultiHost<int, int, IdentityUserLoginMultiHost<int, int>, IdentityUserRoleMultiHost<int>, IdentityUserClaimMultiHost<int>>, int, int>
    {
        public UserManagerMultiHostInt(UserStoreMultiHostInt store)
            : this(store, 0)
        {

        }

        public UserManagerMultiHostInt(UserStoreMultiHostInt store, int hostId)
            : base(store, hostId)
        {
            // allow duplicate emails and funky chars
            this.UserValidator = new UserValidator<IdentityUserMultiHost<int, int, IdentityUserLoginMultiHost<int, int>, IdentityUserRoleMultiHost<int>, IdentityUserClaimMultiHost<int>>, int>(this) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = false };
        }
    }

    public class UserManagerMultiHostLong : UserManagerMultiHost<IdentityUserMultiHost<long, long, IdentityUserLoginMultiHost<long, long>, IdentityUserRoleMultiHost<long>, IdentityUserClaimMultiHost<long>>, long, long>
    {
        public UserManagerMultiHostLong(UserStoreMultiHostLong store)
            : this(store, 0)
        {

        }

        public UserManagerMultiHostLong(UserStoreMultiHostLong store, long hostId)
            : base(store, hostId)
        {

        }
    }
}
