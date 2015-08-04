using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHost{TKey}</c>.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
    /// <typeparam name="TUserRole">The type of the user role.</typeparam>
    /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
    public class UserStoreMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : UserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, IUserMultiHost<TKey>, new()
        where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
        where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
        where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
    {
        public TKey HostId { get; private set; }
        public TKey SystemHostId { get; private set; }
        protected UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> UserManager { get; private set; }
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHost{TUser, TKey}"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        public UserStoreMultiHost(DbContext context, TKey systemHostId, TKey hostId)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(!systemHostId.Equals(default(TKey)), "systemHostId");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");

            this.HostId = hostId;
            this.SystemHostId = systemHostId;
            this.UserManager = CreateUserManager();
        }

        protected virtual UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> CreateUserManager()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new user for the host specified in <c>user.HostId</c>. If no host id specified, the default host id is used.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Global users must belong to system host.</exception>
        public override async Task CreateAsync(TUser user)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");

            ThrowIfDisposed();

            if (user.HostId.Equals(default(TKey)))
            {
                user.HostId = this.HostId;
            }

            if (user.IsGlobal && !user.HostId.Equals(this.SystemHostId))
            {
                throw new ArgumentException("Global users must belong to system host.");
            }
            UserManager.Create(user);
            await base.CreateAsync(user);
        }

        /// <summary>
        /// Finds a user belonging to the default host or that is a global user.
        /// </summary>
        /// <param name="login">The user's login info.</param>
        /// <returns></returns>
        public override async Task<TUser> FindAsync(UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(login != null, "login");

            ThrowIfDisposed();

            return await FindAsync(this.HostId, login);
        }

        /// <summary>
        /// Finds a user belonging to the specified host or that is a global user.
        /// </summary>
        /// <param name="hostId">The host id.</param>
        /// <param name="login">The user's login info.</param>
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

            // user does not exist, user is not global, or user does not belong to this host
            return null;
        }

        /// <summary>
        /// Add a claim to a user for the current host.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public override async Task AddClaimAsync(TUser user, System.Security.Claims.Claim claim)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            await AddClaimAsync(this.HostId, user, claim);
        }

        /// <summary>
        /// Add a claim to a user for the specified host.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public async Task AddClaimAsync(TKey hostId, TUser user, System.Security.Claims.Claim claim)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            var userClaim = new TUserClaim
            {
                HostId = hostId,
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                IsGlobal = user.IsGlobal
            };

            await Task.Run(() => user.Claims.Add(userClaim));
        }

        /// <summary>
        /// Add a login to the user for the current host. If user is global, login is as well.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(login != null, "login");

            ThrowIfDisposed();

            await AddLoginAsync(this.HostId, user, login);
        }

        /// <summary>
        /// Add a login to the user for the specified host. If user is global, login is as well.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public async Task AddLoginAsync(TKey hostId, TUser user, UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(login != null, "login");

            ThrowIfDisposed();

            var userLogin = new TUserLogin
            {
                HostId = hostId,
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                IsGlobal = user.IsGlobal
            };

            await Task.Run(() => user.Logins.Add(userLogin));
        }

        /// <summary>
        /// Add a user to a role for the current host. Role must belong to current host or be a global role.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public override async Task AddToRoleAsync(TUser user, string roleName)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            await AddToRoleAsync(user.HostId, user, roleName);
        }

        /// <summary>
        /// Add a user to a role for the specified host. Role must belong to specified host or be a global role.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task AddToRoleAsync(TKey hostId, TUser user, string roleName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            var role = await Context.Set<TRole>()
                .Where(r => r.Name == roleName
                    && (r.HostId.Equals(hostId) || r.IsGlobal == true))
                .SingleOrDefaultAsync();

            if (role == null)
            {
                throw new Exception(string.Format("Role '{0}' not found for hostId '{1}' or in global roles.", roleName, hostId));
            }

            var userRole = new TUserRole
            {
                HostId = user.HostId,
                UserId = user.Id,
                RoleId = role.Id,
                IsGlobal = role.IsGlobal
            };

            user.Roles.Add(userRole);
        }

        /// <summary>
        /// Get a users's claims for current host or global.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public override async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");

            ThrowIfDisposed();

            return await GetClaimsAsync(this.HostId, user);
        }

        /// <summary>
        /// Get a users's claims for specified host or global.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<IList<Claim>> GetClaimsAsync(TKey hostId, TUser user)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(user != null, "user");

            ThrowIfDisposed();

            return await Context.Set<TUserClaim>()
                .Where(uc => uc.UserId.Equals(user.Id)
                    && (uc.HostId.Equals(hostId) || uc.IsGlobal))
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToListAsync();
        }

        /// <summary>
        /// Get the names of the roles a user is a member of for the current host; includes global roles.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public override async Task<IList<string>> GetRolesAsync(TUser user)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");

            ThrowIfDisposed();

            return await GetRolesAsync(this.HostId, user);
        }

        /// <summary>
        /// Get the names of the roles a user is a member of for the specified host; includes global roles.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<IList<string>> GetRolesAsync(TKey hostId, TUser user)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(user != null, "user");

            ThrowIfDisposed();

            var roleIds = user.Roles
                .Where(r => r.HostId.Equals(hostId) || r.IsGlobal == true)
                .Select(r => r.RoleId)
                .ToArray();

            return await Context.Set<TRole>()
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Finds a user for the current host (or global) with the given email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns></returns>
        public new async Task<TUser> FindByEmailAsync(string email)
        {
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");

            ThrowIfDisposed();

            return await FindByEmailAsync(this.HostId, email);
        }

        /// <summary>
        /// Finds a user for the given host with the given email address.
        /// </summary>
        /// <param name="hostId">The host id.</param>
        /// <param name="email">The user's email address.</param>
        /// <returns></returns>
        public virtual async Task<TUser> FindByEmailAsync(TKey hostId, string email)
        {
            Contract.Requires<ArgumentException>(!hostId.Equals(default(TKey)));
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");

            ThrowIfDisposed();

            return await Users
                .SingleOrDefaultAsync(u => u.Email == email
                    && (u.HostId.Equals(hostId) || u.IsGlobal));
        }

        /// <summary>
        /// Finds a user for the current host (or global) with the specified userName.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <returns></returns>
        public override async Task<TUser> FindByNameAsync(string userName)
        {
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");

            ThrowIfDisposed();

            return await FindByNameAsync(this.HostId, userName);
        }

        /// <summary>
        /// Finds a user for the given host with the specified userName.
        /// </summary>
        /// <param name="hostId">The host id.</param>
        /// <param name="userName">The userName.</param>
        /// <returns></returns>
        public virtual async Task<TUser> FindByNameAsync(TKey hostId, string userName)
        {
            Contract.Requires<ArgumentException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");

            ThrowIfDisposed();

            return await Users
                .SingleOrDefaultAsync(u => u.UserName == userName
                    && (u.HostId.Equals(hostId) || u.IsGlobal ==  true));
        }

        /// <summary>
        /// Remove a user claim from the current host.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public override async Task RemoveClaimAsync(TUser user, Claim claim)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            await RemoveClaimAsync(this.HostId, user, claim);
        }

        /// <summary>
        /// Remove a user claim from the specified host.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public async Task RemoveClaimAsync(TKey hostId, TUser user, Claim claim)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            ThrowIfDisposed();

            var userClaim = user.Claims
                .Where(c => c.HostId.Equals(hostId)
                    && c.ClaimType == claim.Type
                    && c.ClaimValue == claim.Value)
                .SingleOrDefault();

            if (userClaim != null)
            {
                user.Claims.Remove(userClaim);
            }
        }

        /// <summary>
        /// Remove a user from a role (current host or global).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public override async Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            await RemoveFromRoleAsync(this.HostId, user, roleName);
        }

        /// <summary>
        /// Remove a user from a role (specified host or global).
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public async Task RemoveFromRoleAsync(TKey hostId, TUser user, string roleName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            var role = await FindRoleAsync(this.HostId, roleName);

            var userRole = user.Roles
                .Where(r => r.RoleId.Equals(role.Id)
                    && (r.HostId.Equals(hostId) || r.IsGlobal))
                .SingleOrDefault();

            if (userRole != null)
            {
                user.Roles.Remove(userRole);
            }
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Users cannot be assigned a new hostId.
        /// or
        /// Global users must belong to system host.</exception>
        public override async Task UpdateAsync(TUser user)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");

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

            await base.UpdateAsync(user);
        }

        /// <summary>
        /// Finds a role by name for the current host or in the global roles.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
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
                    UserManager = null;

                    this.disposed = true;
                }
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key of type <c>Guid</c>.
    /// </summary>
    public class UserStoreMultiHostGuid<TUser> : UserStoreMultiHost<TUser, IdentityRoleMultiHostGuid, Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid>
        where TUser : IdentityUserMultiHostGuid, IUserMultiHostGuid, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostGuid" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        /// <param name="systemHostId">The system host identifier.</param>
        public UserStoreMultiHostGuid(DbContext context, Guid systemHostId, Guid hostId)
            : base(context, systemHostId, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(systemHostId != Guid.Empty, "systemHostId");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }

        /// <summary>
        /// Creates the user manager.
        /// </summary>
        /// <returns></returns>
        protected override UserManagerMultiHost<TUser, IdentityRoleMultiHostGuid, Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid> CreateUserManager()
        {
            return new UserManagerMultiHostGuid<TUser>(this);
        }

        /// <summary>
        /// Gets a <c>UserManager</c>.
        /// </summary>
        /// <returns></returns>
        public UserManagerMultiHostGuid<TUser> GetUserManager()
        {
            return (UserManagerMultiHostGuid<TUser>)UserManager;
        }
    }

    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key of type <c>int</c>.
    /// </summary>
    public class UserStoreMultiHostInt<TUser> : UserStoreMultiHost<TUser, IdentityRoleMultiHostInt, int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt>
        where TUser : IdentityUserMultiHostInt, IUserMultiHostInt, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostInt" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        /// <param name="systemHostId">The system host identifier.</param>
        public UserStoreMultiHostInt(DbContext context, int systemHostId, int hostId)
            : base(context, systemHostId, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(systemHostId > 0);
            Contract.Requires<ArgumentException>(hostId > 0);
        }

        /// <summary>
        /// Creates the user manager.
        /// </summary>
        /// <returns></returns>
        protected override UserManagerMultiHost<TUser, IdentityRoleMultiHostInt, int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt> CreateUserManager()
        {
            return new UserManagerMultiHostInt<TUser>(this);
        }

        /// <summary>
        /// Gets a <c>UserManager</c>.
        /// </summary>
        /// <returns></returns>
        public UserManagerMultiHostInt<TUser> GetUserManager()
        {
            return (UserManagerMultiHostInt<TUser>)UserManager;
        }
    }

    /// <summary>
    /// EntityFramework <c>UserStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key of type <c>long</c>.
    /// </summary>
    public class UserStoreMultiHostLong<TUser> : UserStoreMultiHost<TUser, IdentityRoleMultiHostLong, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong>
        where TUser : IdentityUserMultiHostLong, IUserMultiHostLong, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStoreMultiHostLong" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id.</param>
        /// <param name="systemHostId">The system host identifier.</param>
        public UserStoreMultiHostLong(DbContext context, long systemHostId, long hostId)
            : base(context, systemHostId, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(systemHostId > 0);
            Contract.Requires<ArgumentException>(hostId > 0);
        }

        /// <summary>
        /// Creates the user manager.
        /// </summary>
        /// <returns></returns>
        protected override UserManagerMultiHost<TUser, IdentityRoleMultiHostLong, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong> CreateUserManager()
        {
            return new UserManagerMultiHostLong<TUser>(this);
        }

        /// <summary>
        /// Gets a <c>UserManager</c>.
        /// </summary>
        /// <returns></returns>
        public UserManagerMultiHostLong<TUser> GetUserManager()
        {
            return (UserManagerMultiHostLong<TUser>)UserManager;
        }
    }
}
