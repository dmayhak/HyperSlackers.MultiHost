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
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> which will automatically save changes to the <c>RoleStore</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="TRole">A role type derived from <c>IdentityRoleMultiHost{TKey, THostKey}</c>.</typeparam>
    public class RoleManagerMultiHost<TKey, THostKey, TRole> : RoleManager<TRole, TKey>
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
        /// Initializes a new instance of the <see cref="RoleManagerMultiHost{TKey, THostKey, TRole}"/> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        public RoleManagerMultiHost(RoleStoreMultiHost<TKey, THostKey, TRole> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHost{TKey, THostKey, TRole}"/> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        /// <param name="hostId">The default host id to use when host id not specified.</param>
        public RoleManagerMultiHost(RoleStoreMultiHost<TKey, THostKey, TRole> store, THostKey hostId)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            this.HostId = hostId;
        }

        /// <summary>
        /// Checks if a role exists for the default host.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns><c>true</c> if role exists, otherwise, <c>false</c></returns>
        public override async Task<bool> RoleExistsAsync(string roleName)
        {
            //x Contract.Requires<ArgumentException>(!roleName.IsNullOrWhiteSpace());

            return await Task.FromResult(RoleExists(roleName, this.HostId));
        }

        /// <summary>
        /// Checks if a role exists for the specified host.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns>
        ///   <c>true</c> if role exists, otherwise, <c>false</c>
        /// </returns>
        public bool RoleExists(string roleName, THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!roleName.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            return Roles.Any(r => r.Name == roleName && r.HostId.Equals(hostId));
        }

        /// <summary>
        /// Creates a role for the host specified in <c>role.HostId</c>. If host id not specified, the default host is used.
        /// </summary>
        /// <param name="user">The user.</param>
        public override Task<IdentityResult> CreateAsync(TRole role)
        {
            if (EqualityComparer<THostKey>.Default.Equals(role.HostId, default(THostKey)))
            {
                role.HostId = this.HostId;
            }

            return base.CreateAsync(role);
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
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>string</c> data types.
    /// </summary>
    public class RoleManagerMultiHostString : RoleManagerMultiHost<string, string, IdentityRoleMultiHost<string, string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostString" /> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        public RoleManagerMultiHostString(RoleStoreMultiHostString store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostString"/> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleManagerMultiHostString(RoleStoreMultiHostString store, string hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace());
        }
    }

    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>Guid</c> data types.
    /// </summary>
    public class RoleManagerMultiHostGuid : RoleManagerMultiHost<Guid, Guid, IdentityRoleMultiHost<Guid, Guid>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostGuid" /> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        public RoleManagerMultiHostGuid(RoleStoreMultiHostGuid store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostGuid"/> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleManagerMultiHostGuid(RoleStoreMultiHostGuid store, Guid hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }
    }

    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>int</c> data types.
    /// </summary>
    public class RoleManagerMultiHostInt : RoleManagerMultiHost<int, int, IdentityRoleMultiHost<int, int>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostInt" /> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        public RoleManagerMultiHostInt(RoleStoreMultiHostInt store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostInt"/> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleManagerMultiHostInt(RoleStoreMultiHostInt store, int hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentException>(hostId > 0, "hostId");
        }
    }

    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>long</c> data types.
    /// </summary>
    public class RoleManagerMultiHostLong : RoleManagerMultiHost<long, long, IdentityRoleMultiHost<long, long>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostLong" /> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        public RoleManagerMultiHostLong(RoleStoreMultiHostLong store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHostLong"/> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        /// <param name="hostId">The host id.</param>
        public RoleManagerMultiHostLong(RoleStoreMultiHostLong store, long hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentException>(hostId > 0, "hostId");
        }
    }
}
