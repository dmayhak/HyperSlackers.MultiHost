// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Claims;
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> which will automatically save changes to the <c>UserStore</c>.
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHost{TKey, TKey}</c>.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
    /// <typeparam name="TUserRole">The type of the user role.</typeparam>
    /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
    public class UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : UserManager<TUser, TKey>
        where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, IUserMultiHost<TKey>, new()
        where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
        where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
        where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
    {
        protected internal TKey HostId { get; private set; }
        protected internal TKey SystemHostId { get; private set; }
        protected DbContext Context = null;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHost{TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim}"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public UserManagerMultiHost(UserStoreMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");

            // allow duplicate emails and funky chars
            this.UserValidator = new UserValidator<TUser, TKey>(this) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = false };

            this.Context = store.Context;
            this.HostId = store.HostId;
            this.SystemHostId = store.SystemHostId;
        }

        /// <summary>
        /// Add a user claim for the current host.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> AddClaimAsync(TKey userId, System.Security.Claims.Claim claim)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            return await AddClaimAsync(this.HostId, userId, claim);
        }

        /// <summary>
        /// Add a user claim for the specified host.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public async Task<IdentityResult> AddClaimAsync(TKey hostId, TKey userId, System.Security.Claims.Claim claim)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            var user = await FindByIdAsync(userId);

            var userClaim = new TUserClaim()
            {
                UserId = userId,
                HostId = hostId,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                IsGlobal = user.IsGlobal
            };

            user.Claims.Add(userClaim);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Associate a login with a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> AddLoginAsync(TKey userId, UserLoginInfo login)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentNullException>(login != null, "login");

            ThrowIfDisposed();

            var user = await FindByIdAsync(userId);

            var userLogin = new TUserLogin()
            {
                UserId = userId,
                HostId = user.HostId,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                IsGlobal = user.IsGlobal
            };

            user.Logins.Add(userLogin);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Finds a role by name for the current host or in the global roles.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        protected virtual async Task<TRole> FindRoleAsync(string roleName)
        {
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await FindRoleAsync(this.HostId, roleName);
        }

        /// <summary>
        /// Finds a role by name for the specified host or in the global roles.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="roleName">The role.</param>
        /// <returns></returns>
        protected virtual async Task<TRole> FindRoleAsync(TKey hostId, string roleName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await Context.Set<TRole>()
                .Where(r => r.Name == roleName
                    && (r.HostId.Equals(hostId) || r.IsGlobal == true))
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Add a user to a role for the current host. roleName must belong to current host or be a global role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> AddToRoleAsync(TKey userId, string roleName)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await AddToRoleAsync(this.HostId, userId, roleName);
        }

        /// <summary>
        /// Add a user to a role for the specified host. roleName must belong to specified host or be a global role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <returns></returns>
        public async Task<IdentityResult> AddToRoleAsync(TKey hostId, TKey userId, string roleName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            var user = await FindByIdAsync(userId);
            var role = await FindRoleAsync(hostId, roleName);

            if (role == null)
            {
                throw new Exception(string.Format("Role '{0}' not found for hostId '{1}' or in global roles.", roleName, hostId));
            }

            var userRole = new TUserRole()
            {
                UserId = userId,
                RoleId = role.Id,
                HostId = hostId,
                IsGlobal = role.IsGlobal
            };

            user.Roles.Add(userRole);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Method to add user to multiple roles for the current host. Role names must belong to
        /// the current host or be a global role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleNames">The roles.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> AddToRolesAsync(TKey userId, params string[] roleNames)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentNullException>(roleNames != null, "roleNames");

            ThrowIfDisposed();

            return await AddToRolesAsync(this.HostId, userId, roleNames);
        }

        /// <summary>
        /// Method to add user to multiple roles.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleNames">The roles.</param>
        /// <returns></returns>
        public async Task<IdentityResult> AddToRolesAsync(TKey hostId, TKey userId, params string[] roleNames)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(roleNames != null, "roleNames");

            ThrowIfDisposed();

            foreach (var roleName in roleNames)
            {
                await AddToRoleAsync(hostId, userId, roleName);
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Create a user with no password for the host specified in user.HostId, or current host if not specified.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> CreateAsync(TUser user)
        {
            //x Contract.Requires<ArgumentNullException>(user != null, "user");

            ThrowIfDisposed();

            if (user.HostId.Equals(default(TKey)))
            {
                user.HostId = this.HostId;
            }

            if (user.IsGlobal && !user.HostId.Equals(this.SystemHostId))
            {
                throw new ArgumentException("Global users must belong to system host.");
            }

            return await base.CreateAsync(user);
        }

        /// <summary>
        /// Create a user with the given password for the host specified in user.HostId, or current host if not specified.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            //x Contract.Requires<ArgumentNullException>(user != null, "user");
            //x Contract.Requires<ArgumentNullException>(!password.IsNullOrWhiteSpace(), "password");

            ThrowIfDisposed();

            if (user.HostId.Equals(default(TKey)))
            {
                user.HostId = this.HostId;
            }

            if (user.IsGlobal && !user.HostId.Equals(this.SystemHostId))
            {
                throw new ArgumentException("Global users must belong to system host.");
            }

            return await base.CreateAsync(user, password);
        }

        /// <summary>
        /// Creates a ClaimsIdentity representing the user for the host specified in user.HostId, or current host if not specified.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="authenticationType">the authentication type.</param>
        /// <returns></returns>
        public override async Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType)
        {
            //x Contract.Requires<ArgumentNullException>(user != null, "user");
            //x Contract.Requires<ArgumentNullException>(!authenticationType.IsNullOrWhiteSpace(), "authenticationType");

            ThrowIfDisposed();

            // TODO: any custom logic here?

            return await base.CreateIdentityAsync(user, authenticationType);
        }

        /// <summary>
        /// Returns the user associated with this login for the current host or is a global user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override async Task<TUser> FindAsync(string userName, string password)
        {
            //x Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");
            //x Contract.Requires<ArgumentNullException>(!password.IsNullOrWhiteSpace(), "password");

            ThrowIfDisposed();

            return await FindAsync(this.HostId, userName, password);
        }

        /// <summary>
        /// Returns the user associated with this login for the specified host or system host if specified.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<TUser> FindAsync(TKey hostId, string userName, string password, bool includeSystemHost = false)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");
            Contract.Requires<ArgumentNullException>(!password.IsNullOrWhiteSpace(), "password");

            ThrowIfDisposed();

            var hash = PasswordHasher.HashPassword(password);

            return await Users
                .Where(u => u.UserName == userName
                    && u.PasswordHash == hash
                    && (u.HostId.Equals(hostId) || u.HostId.Equals(this.SystemHostId)))
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Returns the user associated with this login. Only users for the current host or global users are returned.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public override async Task<TUser> FindAsync(UserLoginInfo login)
        {
            //x Contract.Requires<ArgumentNullException>(login != null, "login");

            ThrowIfDisposed();

            return await FindAsync(this.HostId, login);
        }

        /// <summary>
        /// Returns the user associated with this login. Only users for the specified host or global users are returned.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public async Task<TUser> FindAsync(TKey hostId, UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(login != null, "login");

            ThrowIfDisposed();

            var userId = await Context.Set<TUserLogin>()
                .Where(l => l.LoginProvider == login.LoginProvider
                    && l.ProviderKey == login.ProviderKey
                    && (l.HostId.Equals(hostId) || l.IsGlobal))
                .Select(l => l.UserId)
                .SingleOrDefaultAsync();

            if (!userId.Equals(default(TKey)))
            {
                return await FindByIdAsync(userId);
            }

            return null;
        }

        /// <summary>
        /// Find a user by their email. Only users for the current host or global users are returned.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public override async Task<TUser> FindByEmailAsync(string email)
        {
            //x Contract.Requires<ArgumentException>(!email.IsNullOrWhiteSpace());

            ThrowIfDisposed();

            return await FindByEmailAsync(this.HostId, email);
        }

        /// <summary>
        /// Find a user by their email. Only users for the specified host or global users are returned.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public async Task<TUser> FindByEmailAsync(TKey hostId, string email)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentException>(!email.IsNullOrWhiteSpace());

            ThrowIfDisposed();

            return await Users
                .Where(u => u.Email == email
                    && (u.HostId.Equals(hostId) || u.IsGlobal))
                    .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Find a user by user name. Only users for the current host or global users are returned.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public override async Task<TUser> FindByNameAsync(string userName)
        {
            //x Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            ThrowIfDisposed();

            return await FindByNameAsync(this.HostId, userName);
        }

        /// <summary>
        /// Find a user by user name. Only users for the specified host or global users are returned.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public async Task<TUser> FindByNameAsync(TKey hostId, string userName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            ThrowIfDisposed();

            return await Users
                .Where(u => u.UserName == userName
                    && (u.HostId.Equals(hostId) || u.IsGlobal))
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Returns all the roles for the user for all hosts.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<IList<TRole>> GetAllRolesAsync(TKey userId)
        {
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");

            ThrowIfDisposed();

            var roleIds = await Context.Set<TUserRole>()
                 .Where(ur => ur.UserId.Equals(userId))
                 .Select(ur => ur.RoleId)
                 .ToArrayAsync();

            var roles = await Context.Set<TRole>()
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

            return roles;
        }

        /// <summary>
        /// Returns the current host and global roles for the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public override async Task<IList<string>> GetRolesAsync(TKey userId)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");

            ThrowIfDisposed();

            return await GetRolesAsync(this.HostId, userId);
        }

        /// <summary>
        /// Returns the host and global roles for the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<IList<string>> GetRolesAsync(TKey hostId, TKey userId)
        {
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");

            ThrowIfDisposed();

            var roleIds = await Context.Set<TUserRole>()
                .Where(ur => ur.UserId.Equals(userId)
                    && (ur.HostId.Equals(hostId) || ur.IsGlobal))
                .Select(ur => ur.RoleId)
                .ToArrayAsync();

            var roleNames = await Context.Set<TRole>()
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            return roleNames;
        }

        /// <summary>
        /// Determines whether the specified user is in the specified role. Checks global roles and current host roles.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public override async Task<bool> IsInRoleAsync(TKey userId, string roleName)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await IsInRoleAsync(this.HostId, userId, roleName);
        }

        /// <summary>
        /// Determines whether the specified user is in the specified role. Checks global roles and specified host roles.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns><c>true</c> if user is in the role, <c>false</c> otherwise</returns>
        public async Task<bool> IsInRoleAsync(TKey hostId, TKey userId, string roleName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            var roles = await GetRolesAsync(hostId, userId);

            return roles.Contains(roleName);
        }

        /// <summary>
        /// Get a users's claims for current host or global.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public override async Task<IList<Claim>> GetClaimsAsync(TKey userId)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");

            ThrowIfDisposed();

            return await GetClaimsAsync(this.HostId, userId);
        }

        /// <summary>
        /// Get a users's claims for specified host or global.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<IList<Claim>> GetClaimsAsync(TKey hostId, TKey userId)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");

            ThrowIfDisposed();

            return await Context.Set<TUserClaim>()
                .Where(uc => uc.UserId.Equals(userId)
                    && (uc.HostId.Equals(hostId) || uc.IsGlobal))
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToListAsync();
        }

        /// <summary>
        /// Remove a user claim from the current host.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> RemoveClaimAsync(TKey userId, Claim claim)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            return await RemoveClaimAsync(this.HostId, userId, claim);
        }

        /// <summary>
        /// Remove a user claim from the specified host.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveClaimAsync(TKey hostId, TKey userId, Claim claim)
        {
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            var user = await FindByIdAsync(userId);

            var userClaim = user.Claims
                .Where(c => c.HostId.Equals(hostId)
                    && c.ClaimType == claim.Type
                    && c.ClaimValue == claim.Value)
                .SingleOrDefault();

            if (userClaim != null)
            {
                user.Claims.Remove(userClaim);
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Remove a user from a role (current host or global).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> RemoveFromRoleAsync(TKey userId, string role)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentException>(!role.IsNullOrWhiteSpace());

            ThrowIfDisposed();

            return await RemoveFromRoleAsync(this.HostId, userId, role);
        }

        /// <summary>
        /// Remove a user from a role (specified host or global).
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveFromRoleAsync(TKey hostId, TKey userId, string roleName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentException>(!roleName.IsNullOrWhiteSpace());

            ThrowIfDisposed();

            var user = await FindByIdAsync(userId);
            var role = await FindRoleAsync(this.HostId, roleName);
            var userRole = user.Roles
                .Where(r => r.RoleId.Equals(role.Id)
                    && (r.HostId.Equals(hostId) || r.IsGlobal))
                .SingleOrDefault();

            if (userRole != null)
            {
                user.Roles.Remove(userRole);
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Remove user from multiple roles (current host or global).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roles">The roles.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> RemoveFromRolesAsync(TKey userId, params string[] roles)
        {
            //x Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            //x Contract.Requires<ArgumentNullException>(roles != null, "roles");

            ThrowIfDisposed();

            return await RemoveFromRolesAsync(this.HostId, userId, roles);
        }

        /// <summary>
        /// Remove user from multiple roles (specified host or global).
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roles">The roles.</param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveFromRolesAsync(TKey hostId, TKey userId, params string[] roles)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(roles != null, "roles");

            ThrowIfDisposed();

            foreach (var role in roles)
            {
                await RemoveFromRoleAsync(hostId, userId, role);
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Users cannot be assigned a new hostId.
        /// or
        /// Global users must belong to system host.
        /// </exception>
        public override async Task<IdentityResult> UpdateAsync(TUser user)
        {
            //x Contract.Requires<ArgumentNullException>(user != null, "user");

            ThrowIfDisposed();

            var existing = await FindByIdAsync(user.Id);

            if (!user.HostId.Equals(existing.HostId))
            {
                throw new ArgumentException("Users cannot be assigned a new hostId.");
            }

            if (user.IsGlobal && !user.HostId.Equals(this.SystemHostId))
            {
                throw new ArgumentException("Global users must belong to system host.");
            }

            return await base.UpdateAsync(user);
        }

        private IUserClaimStore<TUser, TKey> GetClaimStore()
        {
            var cast = Store as IUserClaimStore<TUser, TKey>;

            if (cast == null)
            {
                throw new NotSupportedException("Store not an IUserClaimStore");
            }

            return cast;
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // TODO: cache references? if so, release them here
                    this.Context = null;

                    this.disposed = true;
                }
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>string</c>.
    /// </summary>
    public class UserManagerMultiHostString<TUser> : UserManagerMultiHost<TUser, IdentityRoleMultiHostString, string, IdentityUserLoginMultiHostString, IdentityUserRoleMultiHostString, IdentityUserClaimMultiHostString>
        where TUser : IdentityUserMultiHostString, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostString" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostString(UserStoreMultiHostString<TUser> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }
    }

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>Guid</c>.
    /// </summary>
    public class UserManagerMultiHostGuid<TUser> : UserManagerMultiHost<TUser, IdentityRoleMultiHostGuid, Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid>
        where TUser : IdentityUserMultiHostGuid, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostGuid" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostGuid(UserStoreMultiHostGuid<TUser> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }
    }

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>int</c>.
    /// </summary>
    public class UserManagerMultiHostInt<TUser> : UserManagerMultiHost<TUser, IdentityRoleMultiHostInt, int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt>
        where TUser : IdentityUserMultiHostInt, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostInt" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostInt(UserStoreMultiHostInt<TUser> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }
    }

    /// <summary>
    /// Exposes user related API for a multi-tenant <c>DbContext</c> having key and host key of type <c>long</c>.
    /// </summary>
    public class UserManagerMultiHostLong<TUser> : UserManagerMultiHost<TUser, IdentityRoleMultiHostLong, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong>
        where TUser : IdentityUserMultiHostLong, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerMultiHostLong" /> class.
        /// </summary>
        /// <param name="store">The <c>UserStore</c>.</param>
        public UserManagerMultiHostLong(UserStoreMultiHostLong<TUser> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }
    }
}
