using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    public class RoleStoreMultiHost<TKey, THostKey, TRole> : RoleStore<TRole, TKey, IdentityUserRoleMultiHost<TKey>>, IQueryableRoleStore<TRole, TKey>, IRoleStore<TRole, TKey>, IDisposable
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
        where TRole : IdentityRoleMultiHost<TKey, THostKey>, new()
    {
        public virtual THostKey HostId { get; set; }
        private bool disposed = false;

        public RoleStoreMultiHost(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public RoleStoreMultiHost(DbContext context, THostKey hostId)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");

            this.HostId = hostId;
        }

        public IQueryable<TRole> AllForHost(THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            return this.Roles.Where(r => r.HostId.Equals(hostId));
        }

        public override async Task CreateAsync(TRole role)
        {
            //x Contract.Requires<ArgumentNullException>(role != null, "role");

            if (EqualityComparer<THostKey>.Default.Equals(role.HostId, default(THostKey)))
            {
                // if host id not specified on user, assign current one
                role.HostId = this.HostId;
            }

            await base.CreateAsync(role);
        }

        public async Task CreateAsync(TKey id, THostKey hostId, string roleName)
        {
            TRole role = new TRole() { Id = id, HostId = hostId, Name = roleName };

            await CreateAsync(role);
        }

        public new async Task<TRole> FindByNameAsync(string name)
        {
            //x Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());

            return await FindByNameAsync(name, this.HostId);
        }

        public async Task<TRole> FindByNameAsync(string name, THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            return await this.Roles.Where(r => r.Name == name && r.HostId.Equals(hostId)).SingleOrDefaultAsync();
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

    public class RoleStoreMultiHostString : RoleStoreMultiHost<string, string, IdentityRoleMultiHost<string, string>>
    {
        public RoleStoreMultiHostString(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public RoleStoreMultiHostString(DbContext context, string hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace());
        }
    }

    public class RoleStoreMultiHostGuid : RoleStoreMultiHost<Guid, Guid, IdentityRoleMultiHost<Guid, Guid>>
    {
        public RoleStoreMultiHostGuid(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public RoleStoreMultiHostGuid(DbContext context, Guid hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }
    }

    public class RoleStoreMultiHostInt : RoleStoreMultiHost<int, int, IdentityRoleMultiHost<int, int>>
    {
        public RoleStoreMultiHostInt(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public RoleStoreMultiHostInt(DbContext context, int hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0);
        }
    }

    public class RoleStoreMultiHostLong : RoleStoreMultiHost<long, long, IdentityRoleMultiHost<long, long>>
    {
        public RoleStoreMultiHostLong(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        public RoleStoreMultiHostLong(DbContext context, long hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0);
        }
    }
}
