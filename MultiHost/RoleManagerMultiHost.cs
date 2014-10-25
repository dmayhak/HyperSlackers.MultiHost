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
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> which will automatically save changes to the <c>RoleStore</c>.
    /// </summary>
    /// <typeparam name="TRole">A role type derived from <c>IdentityRoleMultiHost{TKey}</c>.</typeparam>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="TUserRole">The type of the user role.</typeparam>
    public class RoleManagerMultiHost<TRole, TKey, TUserRole> : RoleManager<TRole, TKey>
        where TKey : IEquatable<TKey>
        where TRole :IdentityRoleMultiHost<TKey, TUserRole>, Microsoft.AspNet.Identity.IRole<TKey>, IRoleMultiHost<TKey>, new()
        where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
    {
        public TKey HostId { get; private set; }
        public TKey SystemHostId { get; private set; }
        protected DbContext Context { get; private set; }
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerMultiHost{TKey, TRole}"/> class.
        /// </summary>
        /// <param name="store">The <c>RoleStore</c>.</param>
        public RoleManagerMultiHost(RoleStoreMultiHost<TRole, TKey, TUserRole> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");

            // new role validator to allow duplicate names--just not for same host
            this.RoleValidator = new RoleValidatorMultiHost<TRole, TKey, TUserRole>(this);

            this.Context = store.Context;
            this.HostId = store.HostId;
            this.SystemHostId = store.SystemHostId;
        }

        /// <summary>
        /// Creates a role for the current host
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Global roles must belong to system host.</exception>
        public async Task<IdentityResult> CreateAsync(string roleName, bool global = false)
        {
            //x Contract.Requires<ArgumentNullException>(role != null, "role");

            ThrowIfDisposed();

            if (global)
            {
                return await CreateAsync(this.SystemHostId, roleName, global);
            }
            else
            {
                return await CreateAsync(this.HostId, roleName, global);
            }
        }

        /// <summary>
        /// Creates a role for the current host
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Global roles must belong to system host.</exception>
        public async Task<IdentityResult> CreateAsync(TKey hostId, string roleName, bool global = false)
        {
            Contract.Requires<ArgumentNullException>(hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            var role = new TRole()
            {
                HostId = hostId,
                Name = roleName,
                IsGlobal = global
            };

            return await CreateAsync(role);
        }

        /// <summary>
        /// Creates a role for the host specified in <c>role.HostId</c> or the current host if unspecified.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Global roles must belong to system host.</exception>
        public override async Task<IdentityResult> CreateAsync(TRole role)
        {
            //x Contract.Requires<ArgumentNullException>(role != null, "role");

            ThrowIfDisposed();

            if (role.HostId.Equals(default(TKey)))
            {
                role.HostId = this.HostId;
            }

            if (role.IsGlobal && !role.HostId.Equals(this.SystemHostId))
            {
                throw new ArgumentException("Global roles must belong to system host.");
            }

            return await base.CreateAsync(role);
        }

        /// <summary>
        /// Find a role by name for the current host or global role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public override async Task<TRole> FindByNameAsync(string roleName)
        {
            //x Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await FindByNameAsync(this.HostId, roleName);
        }

        /// <summary>
        ///  Find a role by name. Searches in the specified host, then global roles.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public async Task<TRole> FindByNameAsync(TKey hostId, string roleName)
        {
            Contract.Requires<ArgumentNullException>(hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await Task.FromResult(Roles.SingleOrDefault(r => r.Name == roleName && (r.HostId.Equals(hostId) || r.IsGlobal == true)));
        }

        /// <summary>
        /// Checks if a role exists for the default host or in global roles.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns><c>true</c> if role exists, otherwise, <c>false</c></returns>
        public override async Task<bool> RoleExistsAsync(string roleName)
        {
            //x Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await RoleExistsAsync(this.HostId, roleName);
        }

        /// <summary>
        /// Checks if a role exists for the specified host or in global roles.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="hostId">The host id.</param>
        /// <returns>
        ///   <c>true</c> if role exists, otherwise, <c>false</c>
        /// </returns>
        public async Task<bool> RoleExistsAsync(TKey hostId, string roleName)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

            return await Task.FromResult(Roles.Any(r => r.Name == roleName && (r.HostId.Equals(hostId) || r.IsGlobal == true)));
        }

        /// <summary>
        /// Update an existing role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Roles cannot be assigned a new hostId.</exception>
        /// <exception cref="System.ArgumentException">Global roles must belong to system host.</exception>
        public override async Task<IdentityResult> UpdateAsync(TRole role)
        {
            //x Contract.Requires<ArgumentNullException>(role != null, "role");

            ThrowIfDisposed();

            var existing = await FindByIdAsync(role.Id);

            if (!role.HostId.Equals(existing.HostId))
            {
                throw new ArgumentException("Roles cannot be assigned a new hostId.");
            }

            if (role.IsGlobal && !role.HostId.Equals(this.SystemHostId))
            {
                throw new ArgumentException("Global roles must belong to system host.");
            }

            return await base.UpdateAsync(role);
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
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>string</c> data types.
    /// </summary>
    public class RoleManagerMultiHostString : RoleManagerMultiHost<IdentityRoleMultiHostString, string, IdentityUserRoleMultiHostString>
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
    }

    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>Guid</c> data types.
    /// </summary>
    public class RoleManagerMultiHostGuid : RoleManagerMultiHost<IdentityRoleMultiHostGuid, Guid, IdentityUserRoleMultiHostGuid>
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
    }

    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>int</c> data types.
    /// </summary>
    public class RoleManagerMultiHostInt : RoleManagerMultiHost<IdentityRoleMultiHostInt, int, IdentityUserRoleMultiHostInt>
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
    }

    /// <summary>
    /// Exposes role related API for a multi-tenant <c>DbContext</c> having key and host key as <c>long</c> data types.
    /// </summary>
    public class RoleManagerMultiHostLong : RoleManagerMultiHost<IdentityRoleMultiHostLong, long, IdentityUserRoleMultiHostLong>
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
    }
}
