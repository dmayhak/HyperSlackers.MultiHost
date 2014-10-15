// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSlackers.MultiHost.Extensions;
using Microsoft.AspNet.Identity;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHost{TKey, THostKey}</c>.</typeparam>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class UserStoreMultiHost<TUser, TKey, THostKey> : UserStore<TUser, IdentityRoleMultiHost<TKey, THostKey>, TKey, IdentityUserLoginMultiHost<TKey, THostKey>, IdentityUserRoleMultiHost<TKey>, IdentityUserClaimMultiHost<TKey>>
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
        public THostKey HostId { get; set; }
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHost{TUser, TKey, THostKey}"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public UserStoreMultiHost(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHost{TUser, TKey, THostKey}"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        public UserStoreMultiHost(DbContext context, THostKey hostId)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId is invalid.");

            this.HostId = hostId;
        }

        /// <summary>
        /// Adds a <c>UserLogin</c> to an existing user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login info.</param>
        /// <returns></returns>
        public override Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(login != null, "login");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(user.HostId, default(THostKey)), "User.HostId");

            var userLogin = new IdentityUserLoginMultiHost<TKey, THostKey>
            {
                HostId = user.HostId,
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
            };

            user.Logins.Add(userLogin);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Creates a new user for the host specified in <c>user.HostId</c>. If no host id specified, the default host id is used.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public override async Task CreateAsync(TUser user)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");

            if (EqualityComparer<THostKey>.Default.Equals(user.HostId, default(THostKey)))
            {
                // if host id not specified on user, assign current one
                // TODO: what about site-wide users (hostId = Guid.Empty for example)?
                user.HostId = this.HostId;
            }

            await base.CreateAsync(user);
        }

        /// <summary>
        /// Finds a user for the default host.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <returns></returns>
        public override async Task<TUser> FindAsync(UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(login != null, "login");

            return await FindAsync(login, this.HostId);
        }

        /// <summary>
        /// Finds a user for the specified host.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns></returns>
        public async Task<TUser> FindAsync(UserLoginInfo login, THostKey hostId)
        {
            Contract.Requires<ArgumentNullException>(login != null, "login");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            TKey userId = FindUserId(login, hostId);

            if (EqualityComparer<TKey>.Default.Equals(userId, default(TKey)))
            {
                return null;
            }

            return await Context.Set<TUser>().FindAsync(userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds the user id for the given login info and host.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns>The user's id</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal virtual TKey FindUserId(UserLoginInfo login, THostKey hostId)
        {
            Contract.Requires<ArgumentNullException>(login != null, "login");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds a user for the default host with the given email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns></returns>
        public new async Task<TUser> FindByEmailAsync(string email)
        {
            Contract.Requires<ArgumentException>(!email.IsNullOrWhiteSpace());

            return await FindByEmailAsync(email, this.HostId);
        }

        /// <summary>
        /// Finds a user for the given host with the given email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns></returns>
        public virtual async Task<TUser> FindByEmailAsync(string email, THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!email.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)));

            return await Users.SingleOrDefaultAsync(u => u.Email == email && u.HostId.Equals(hostId));
        }

        /// <summary>
        /// Finds a user for the default host with the specified userName.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <returns></returns>
        public override async Task<TUser> FindByNameAsync(string userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            return await FindByNameAsync(userName, this.HostId);
        }

        /// <summary>
        /// Finds a user for the given host with the specified userName.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns></returns>
        public virtual async Task<TUser> FindByNameAsync(string userName, THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            return await Users.SingleOrDefaultAsync(u => u.UserName == userName && u.HostId.Equals(hostId));
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
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key of type <c>string</c>.
    /// </summary>
    public class UserStoreMultiHostString : UserStoreMultiHost<IdentityUserMultiHost<string, string>, string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostString" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public UserStoreMultiHostString(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostString"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The host id.</param>
        public UserStoreMultiHostString(DbContext context, string hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace()); // TODO: what about a "default" host
        }

        /// <summary>
        /// Gets a <c>UserManager</c>.
        /// </summary>
        /// <returns></returns>
        public UserManagerMultiHostString GetUserManager()
        {
            return new UserManagerMultiHostString(this);
        }

        /// <summary>
        /// Finds the user id for the given login info and host.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns>
        /// The user's id
        /// </returns>
        internal override string FindUserId(UserLoginInfo login, string hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostString>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }

    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key of type <c>Guid</c>.
    /// </summary>
    public class UserStoreMultiHostGuid : UserStoreMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid>
    {
        public UserStoreMultiHostGuid(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostGuid"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        public UserStoreMultiHostGuid(DbContext context, Guid hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId"); // TODO: what about a "default" host
        }

        /// <summary>
        /// Gets a <c>UserManager</c>.
        /// </summary>
        /// <returns></returns>
        public UserManagerMultiHostGuid GetUserManager()
        {
            return new UserManagerMultiHostGuid(this);
        }

        /// <summary>
        /// Finds the user id for the given login info and host.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <param name="hostId">The default host id.</param>
        /// <returns>
        /// The user's id
        /// </returns>
        internal override Guid FindUserId(UserLoginInfo login, Guid hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostGuid>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }

    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key of type <c>int</c>.
    /// </summary>
    public class UserStoreMultiHostInt : UserStoreMultiHost<IdentityUserMultiHost<int, int>, int, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostInt" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public UserStoreMultiHostInt(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostInt"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        public UserStoreMultiHostInt(DbContext context, int hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0); // TODO: what about a "default" host
        }

        /// <summary>
        /// Gets a <c>UserManager</c>.
        /// </summary>
        /// <returns></returns>
        public UserManagerMultiHostInt GetUserManager()
        {
            return new UserManagerMultiHostInt(this);
        }

        /// <summary>
        /// Finds the user id for the given login info and host.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns>
        /// The user's id
        /// </returns>
        internal override int FindUserId(UserLoginInfo login, int hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostInt>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }

    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key of type <c>long</c>.
    /// </summary>
    public class UserStoreMultiHostLong : UserStoreMultiHost<IdentityUserMultiHost<long, long>, long, long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostLong" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public UserStoreMultiHostLong(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostLong"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        public UserStoreMultiHostLong(DbContext context, long hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0); // TODO: what about a "default" host
        }

        /// <summary>
        /// Gets a <c>UserManager</c>.
        /// </summary>
        /// <returns></returns>
        public UserManagerMultiHostLong GetUserManager()
        {
            return new UserManagerMultiHostLong(this);
        }

        /// <summary>
        /// Finds the user id for the given login info and host.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns>
        /// The user's id
        /// </returns>
        internal override long FindUserId(UserLoginInfo login, long hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostLong>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }
}
