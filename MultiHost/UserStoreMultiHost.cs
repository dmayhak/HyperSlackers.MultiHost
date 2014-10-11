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
    public class UserStoreMultiHost<TUser, TKey, THostKey> : UserStore<TUser, IdentityRoleMultiHost<TKey, THostKey>, TKey, IdentityUserLoginMultiHost<TKey, THostKey>, IdentityUserRoleMultiHost<TKey>, IdentityUserClaimMultiHost<TKey>>
        where TUser : IdentityUserMultiHost<TKey, THostKey>, new()
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
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

            TKey userId = FindUserId(login, hostId);

            if (EqualityComparer<TKey>.Default.Equals(userId, default(TKey)))
            {
                return null;
            }

            return await Context.Set<TUser>().FindAsync(userId).ConfigureAwait(false);
        }

        internal virtual TKey FindUserId(UserLoginInfo login, THostKey hostId)
        {
            Contract.Requires<ArgumentNullException>(login != null, "login");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            throw new NotImplementedException();
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
                    // TODO: cache references? if so, release them here

                    this.disposed = true;
                }
            }

            base.Dispose(disposing);
        }
    }

    public class UserStoreMultiHostString : UserStoreMultiHost<IdentityUserMultiHost<string, string>, string, string>
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
            return new UserManagerMultiHostString(this);
        }

        internal override string FindUserId(UserLoginInfo login, string hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostString>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }

    public class UserStoreMultiHostGuid : UserStoreMultiHost<IdentityUserMultiHost<Guid, Guid>, Guid, Guid>
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

        public UserManagerMultiHostGuid GetUserManager()
        {
            return new UserManagerMultiHostGuid(this);
        }

        internal override Guid FindUserId(UserLoginInfo login, Guid hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostGuid>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }

    public class UserStoreMultiHostInt : UserStoreMultiHost<IdentityUserMultiHost<int, int>, int, int>
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

        public UserManagerMultiHostInt GetUserManager()
        {
            return new UserManagerMultiHostInt(this);
        }

        internal override int FindUserId(UserLoginInfo login, int hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostInt>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }

    public class UserStoreMultiHostLong : UserStoreMultiHost<IdentityUserMultiHost<long, long>, long, long>
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

        public UserManagerMultiHostLong GetUserManager()
        {
            return new UserManagerMultiHostLong(this);
        }

        internal override long FindUserId(UserLoginInfo login, long hostId)
        {
            return Context.Set<IdentityUserLoginMultiHostLong>().Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey && l.HostId.Equals(hostId))
                .Select(l => l.UserId)
                .SingleOrDefault();
        }
    }
}
