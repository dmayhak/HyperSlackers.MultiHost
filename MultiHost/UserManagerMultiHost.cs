// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> which will automatically save changes to the <c>UserStore</c>.
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHost{TKey, THostKey}</c>.</typeparam>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class UserManagerMultiHost<TUser, TKey, THostKey> : UserManager<TUser, TKey>
        where TUser : IdentityUserMultiHost<TKey, THostKey>, new()
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        /// <summary>
        /// Gets or sets the host id.
        /// </summary>
        /// <value>
        /// The host id.
        /// </value>
        public virtual THostKey HostId { get; set; }
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHost{TUser, TKey, THostKey}"/> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHost(UserStoreMultiHost<TUser, TKey, THostKey> store)
            : base(store)
        {
            // allow duplicate emails and funky chars
            this.UserValidator = new UserValidator<TUser, TKey>(this) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = false };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHost{TUser, TKey, THostKey}"/> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public UserManagerMultiHost(UserStoreMultiHost<TUser, TKey, THostKey> store, THostKey hostId)
            : this(store)
        {
            this.HostId = hostId;
        }

        /// <summary>
        /// Determines whether the specified user in in the specified role.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns><c>true</c> if user in in the role, <c>false</c> otherwise</returns>
        public bool IsInRole(TKey userId, TKey roleId)
        {
            var user = Store.FindByIdAsync(userId).Result;
            if (user != null)
            {
                return user.Roles.Any(r => r.RoleId.Equals(roleId));
            }

            return false;
        }

        /// <summary>
        /// Removes a user from a role.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roleId">The role id.</param>
        public void RemoveFromRole(TKey userId, TKey roleId)
        {
            var user = Store.FindByIdAsync(userId).Result;
            if (user != null)
            {
                user.Roles.Remove(user.Roles.Where(r => r.RoleId.Equals(roleId)).Single());
            }
        }

        /// <summary>
        /// Creates a user for the host specified in <c>user.HostId</c>. If host id not specified, the default host is used.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The user's password.</param>
        public void CreateUser(TUser user, string password)
        {
            if (EqualityComparer<THostKey>.Default.Equals(user.HostId, default(THostKey)))
            {
                user.HostId = this.HostId;
            }

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

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>string</c>.
    /// </summary>
    public class UserManagerMultiHostString : UserManagerMultiHost<IdentityUserMultiHost<string, string>, string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostString" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostString(UserStoreMultiHost<IdentityUserMultiHost<string, string>, string, string> store)
            : this(store, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostString"/> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public UserManagerMultiHostString(UserStoreMultiHost<IdentityUserMultiHost<string, string>, string, string> store, string hostId)
            : base(store, hostId)
        {
        }
    }

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>Guid</c>.
    /// </summary>
    public class UserManagerMultiHostGuid : UserManagerMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostGuid" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostGuid(UserStoreMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid> store)
            : this(store, Guid.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostGuid"/> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public UserManagerMultiHostGuid(UserStoreMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid> store, Guid hostId)
            : base(store, hostId)
        {
        }
    }

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>int</c>.
    /// </summary>
    public class UserManagerMultiHostInt : UserManagerMultiHost<IdentityUserMultiHost<int, int>, int, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostInt" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostInt(UserStoreMultiHost<IdentityUserMultiHost<int, int>, int, int> store)
            : this(store, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostInt" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public UserManagerMultiHostInt(UserStoreMultiHost<IdentityUserMultiHost<int, int>, int, int> store, int hostId)
            : base(store, hostId)
        {
        }
    }

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>long</c>.
    /// </summary>
    public class UserManagerMultiHostLong : UserManagerMultiHost<IdentityUserMultiHost<long, long>, long, long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostLong" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostLong(UserStoreMultiHost<IdentityUserMultiHost<long, long>, long, long> store)
            : base(store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostLong"/> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public UserManagerMultiHostLong(UserStoreMultiHost<IdentityUserMultiHost<long, long>, long, long> store, long hostId)
            : base(store, hostId)
        {
        }
    }
}
