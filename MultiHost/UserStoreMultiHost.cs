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
    public class UserStoreMultiHost<TUser, TRole, TKey, THostKey, TUserLogin, TUserRole, TUserClaim> : UserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TUser : IdentityUserMultiHost<TKey, THostKey, TUserLogin, TUserRole, TUserClaim>, new()
        where TRole : IdentityRoleMultiHost<TKey, THostKey, TUserRole>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
        where TUserLogin : IdentityUserLoginMultiHost<TKey, THostKey>, new()
        where TUserRole : IdentityUserRoleMultiHost<TKey>, new()
        where TUserClaim : IdentityUserClaimMultiHost<TKey>, new()
    {
        public THostKey HostId { get; set; }
        private bool disposed = false;

        public UserStoreMultiHost(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public UserStoreMultiHost(DbContext context, THostKey hostId)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId is invalid.");

            this.HostId = hostId;
        }

        public override Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(login != null, "login");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(user.HostId, default(THostKey)), "User.HostId");

            var userLogin = new TUserLogin
            {
                HostId = user.HostId,
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
            };

            user.Logins.Add(userLogin);

            return Task.FromResult(0);
        }

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

        public override async Task<TUser> FindAsync(UserLoginInfo login)
        {
            Contract.Requires<ArgumentNullException>(login != null, "login");

            return await FindAsync(login, this.HostId);
        }

        public async Task<TUser> FindAsync(UserLoginInfo login, THostKey hostId)
        {
            Contract.Requires<ArgumentNullException>(login != null, "login");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            TKey userId = await Context.Set<TUserLogin>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (EqualityComparer<TKey>.Default.Equals(userId, default(TKey)))
            {
                return null;
            }

            return await Context.Set<TUser>().FindAsync(userId).ConfigureAwait(false);
        }

        public new async Task<TUser> FindByEmailAsync(string email)
        {
            Contract.Requires<ArgumentException>(!email.IsNullOrWhiteSpace());

            return await FindByEmailAsync(email, this.HostId);
        }

        public virtual async Task<TUser> FindByEmailAsync(string email, THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!email.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)));

            return await Users.SingleOrDefaultAsync(u => u.Email == email && u.HostId.Equals(hostId));
        }

        public override async Task<TUser> FindByNameAsync(string userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            return await FindByNameAsync(userName, this.HostId);
        }

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
                    // anything

                    this.disposed = true;
                }
            }
        }

    }

    //public class UserStoreMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : UserStoreMultiHost<TUser, TRole, TKey, TKey, TUserLogin, TUserRole, TUserClaim>
    //{
    //    public UserStoreMultiHost(DbContext context, TKey hostId)
    //        : base(context, hostId)
    //    {
    //        Contract.Requires<ArgumentNullException>(context != null, "context");
    //    }
    //}

    public class UserStoreMultiHostString
        : UserStoreMultiHost<IdentityUserMultiHost<string, string, IdentityUserLoginMultiHost<string, string>, IdentityUserRoleMultiHost<string>, IdentityUserClaimMultiHost<string>>,
        IdentityRoleMultiHost<string, string, IdentityUserRoleMultiHost<string>>,
        string,
        string,
        IdentityUserLoginMultiHost<string, string>,
        IdentityUserRoleMultiHost<string>,
        IdentityUserClaimMultiHost<string>>
    {
        public UserStoreMultiHostString(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public UserStoreMultiHostString(DbContext context, string hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace()); // TODO: what about a "default" host
        }

        public UserManagerMultiHostString GetUserManager()
        {
            var manager = new UserManagerMultiHostString(this);

            // allow duplicate emails and funky chars
            manager.UserValidator = new UserValidator<IdentityUserMultiHost<string, string, IdentityUserLoginMultiHost<string, string>, IdentityUserRoleMultiHost<string>, IdentityUserClaimMultiHost<string>>, string>(manager) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = false };

            return manager;
        }
    }

    public class UserStoreMultiHostGuid
        : UserStoreMultiHost<IdentityUserMultiHost<Guid, Guid, IdentityUserLoginMultiHost<Guid, Guid>, IdentityUserRoleMultiHost<Guid>, IdentityUserClaimMultiHost<Guid>>,
        IdentityRoleMultiHost<Guid, Guid, IdentityUserRoleMultiHost<Guid>>,
        Guid,
        Guid,
        IdentityUserLoginMultiHost<Guid, Guid>,
        IdentityUserRoleMultiHost<Guid>,
        IdentityUserClaimMultiHost<Guid>>
    {
        public UserStoreMultiHostGuid(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public UserStoreMultiHostGuid(DbContext context, Guid hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId"); // TODO: what about a "default" host
        }
    }

    public class UserStoreMultiHostInt
        : UserStoreMultiHost<IdentityUserMultiHost<int, int, IdentityUserLoginMultiHost<int, int>, IdentityUserRoleMultiHost<int>, IdentityUserClaimMultiHost<int>>,
        IdentityRoleMultiHost<int, int, IdentityUserRoleMultiHost<int>>,
        int,
        int,
        IdentityUserLoginMultiHost<int, int>,
        IdentityUserRoleMultiHost<int>,
        IdentityUserClaimMultiHost<int>>
    {
        public UserStoreMultiHostInt(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public UserStoreMultiHostInt(DbContext context, int hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0); // TODO: what about a "default" host
        }
    }

    public class UserStoreMultiHostLong
        : UserStoreMultiHost<IdentityUserMultiHost<long, long, IdentityUserLoginMultiHost<long, long>, IdentityUserRoleMultiHost<long>, IdentityUserClaimMultiHost<long>>,
        IdentityRoleMultiHost<long, long, IdentityUserRoleMultiHost<long>>,
        long,
        long,
        IdentityUserLoginMultiHost<long, long>,
        IdentityUserRoleMultiHost<long>,
        IdentityUserClaimMultiHost<long>>
    {
        public UserStoreMultiHostLong(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public UserStoreMultiHostLong(DbContext context, long hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0); // TODO: what about a "default" host
        }
    }
}
