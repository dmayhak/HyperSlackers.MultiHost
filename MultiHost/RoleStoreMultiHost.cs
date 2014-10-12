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
    /// <summary>
    /// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="TRole">A role type derived from <c>IdentityRoleMultiHost{TKey, THostKey}</c>.</typeparam>
    public class RoleStoreMultiHost<TKey, THostKey, TRole> : RoleStore<TRole, TKey, IdentityUserRoleMultiHost<TKey>>, IQueryableRoleStore<TRole, TKey>, IRoleStore<TRole, TKey>, IDisposable
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
        where TRole : IdentityRoleMultiHost<TKey, THostKey>, new()
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
        /// Initializes a new instance of the <see cref="RoleStoreMultiHost{TKey, THostKey, TRole}"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public RoleStoreMultiHost(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHost{TKey, THostKey, TRole}"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The default host id to use when host id not specified.</param>
        public RoleStoreMultiHost(DbContext context, THostKey hostId)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");

            this.HostId = hostId;
        }

        /// <summary>
        /// Retrieves all <c>Roles</c> for the given host.
        /// </summary>
        /// <param name="hostId">The host id.</param>
        /// <returns></returns>
        public IQueryable<TRole> AllForHost(THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            return this.Roles.Where(r => r.HostId.Equals(hostId));
        }

        /// <summary>
        /// Creates a new role for the host specified in <c>role.HostId</c>. If no host id specified, the default host id is used.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new role with the given name and id sof the specified host.
        /// </summary>
        /// <param name="id">The new role's id.</param>
        /// <param name="hostId">The host id.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public async Task CreateAsync(TKey id, THostKey hostId, string roleName)
        {
            TRole role = new TRole() { Id = id, HostId = hostId, Name = roleName };

            await CreateAsync(role);
        }

        /// <summary>
        /// Finds a role with the specified name for the default host id.
        /// </summary>
        /// <param name="name">The role ame.</param>
        /// <returns></returns>
        public new async Task<TRole> FindByNameAsync(string name)
        {
            //x Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());

            return await FindByNameAsync(name, this.HostId);
        }

        /// <summary>
        /// Finds a role with the specified name for the specified host id.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns></returns>
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

    /// <summary>
    /// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>string</c>.
    /// </summary>
    public class RoleStoreMultiHostString : RoleStoreMultiHost<string, string, IdentityRoleMultiHost<string, string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostString" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public RoleStoreMultiHostString(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostString"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleStoreMultiHostString(DbContext context, string hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace());
        }
    }

    /// <summary>
    /// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>Guid</c>.
    /// </summary>
    public class RoleStoreMultiHostGuid : RoleStoreMultiHost<Guid, Guid, IdentityRoleMultiHost<Guid, Guid>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostGuid" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public RoleStoreMultiHostGuid(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostGuid"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleStoreMultiHostGuid(DbContext context, Guid hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }
    }

    /// <summary>
    /// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>int</c>.
    /// </summary>
    public class RoleStoreMultiHostInt : RoleStoreMultiHost<int, int, IdentityRoleMultiHost<int, int>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostInt" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public RoleStoreMultiHostInt(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostInt"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleStoreMultiHostInt(DbContext context, int hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0);
        }
    }

    /// <summary>
    /// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>long</c>.
    /// </summary>
    public class RoleStoreMultiHostLong : RoleStoreMultiHost<long, long, IdentityRoleMultiHost<long, long>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostLong" /> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        public RoleStoreMultiHostLong(DbContext context)
            : base(context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStoreMultiHostLong"/> class.
        /// </summary>
        /// <param name="context">The <c>DbContext</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleStoreMultiHostLong(DbContext context, long hostId)
            : base(context, hostId)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentException>(hostId > 0);
        }
    }
}
