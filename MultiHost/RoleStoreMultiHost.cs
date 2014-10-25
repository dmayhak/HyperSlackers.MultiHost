// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

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
	/// <typeparam name="TRole">A role type derived from <c>IdentityRoleMultiHost{TKey}</c>.</typeparam>
	/// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
	/// <typeparam name="TUserRole">The type of the user role.</typeparam>
	public class RoleStoreMultiHost<TRole, TKey, TUserRole> : RoleStore<TRole, TKey, TUserRole>, IQueryableRoleStore<TRole, TKey>, IRoleStore<TRole, TKey>, IDisposable
		where TKey : IEquatable<TKey>
		where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
		where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
	{
		public TKey HostId { get; private set; }
		public TKey SystemHostId { get; private set; }
		protected RoleManagerMultiHost<TRole, TKey, TUserRole> RoleManager { get; private set; }
		private bool disposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="RoleStoreMultiHost{TKey, TKey, TRole}" /> class.
		/// </summary>
		/// <param name="context">The <c>DbContext</c>.</param>
		/// <param name="hostId">The default host id to use when host id not specified.</param>
		/// <param name="systemHostId">The system host identifier (owns global users, roles, etc.)</param>
		public RoleStoreMultiHost(DbContext context, TKey hostId, TKey systemHostId)
			: base(context)
		{
			Contract.Requires<ArgumentNullException>(context != null, "context");
			Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
			Contract.Requires<ArgumentNullException>(!systemHostId.Equals(default(TKey)), "systemHostId");

			this.HostId = hostId;
			this.SystemHostId = systemHostId;
			this.RoleManager = CreateRoleManager();
		}

		protected virtual RoleManagerMultiHost<TRole, TKey, TUserRole> CreateRoleManager()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Retrieves all <c>Roles</c> for the current host and all global roles.
		/// </summary>
		/// <returns></returns>
		public IQueryable<TRole> GetRoles()
        {
            ThrowIfDisposed();

			return GetRoles(this.HostId);
		}

		/// <summary>
		/// Retrieves all <c>Roles</c> for the given host and all global roles.
		/// </summary>
		/// <param name="hostId">The host id.</param>
		/// <returns></returns>
		public IQueryable<TRole> GetRoles(TKey hostId)
		{
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");

            ThrowIfDisposed();

			return this.Roles.Where(r => r.HostId.Equals(hostId) || (r.HostId.Equals(this.SystemHostId) || r.IsGlobal == true));
		}

		/// <summary>
		/// Retrieves all <c>Roles</c>.
		/// </summary>
		/// <returns></returns>
		public IQueryable<TRole> GetAllRoles()
        {
            ThrowIfDisposed();

			return Roles;
		}

		/// <summary>
		/// Creates a new role.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Global roles must belong to system host.</exception>
		public override async Task CreateAsync(TRole role)
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

			await base.CreateAsync(role);
		}

		/// <summary>
		/// Finds a role by name for the current host or in global roles.
		/// </summary>
		/// <param name="roleName">The role name.</param>
		/// <returns></returns>
		public new async Task<TRole> FindByNameAsync(string roleName)
		{
            //x Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

			return await FindByNameAsync(this.HostId, roleName);
		}

		/// <summary>
		/// Finds a role by name for the current host or in global roles.
		/// </summary>
		/// <param name="hostId">The host id.</param>
		/// <param name="roleName">The role name.</param>
		/// <returns></returns>
		public async Task<TRole> FindByNameAsync(TKey hostId, string roleName)
		{
			Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            ThrowIfDisposed();

			return await this.Roles.Where(r => r.Name == roleName && (r.HostId.Equals(hostId) || r.IsGlobal == true)).SingleOrDefaultAsync();
		}

		/// <summary>
		/// Update an existing role.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Roles cannot be assigned a new hostId.</exception>
		/// <exception cref="System.ArgumentException">Global roles must belong to system host.</exception>
		public override async Task UpdateAsync(TRole role)
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

			await base.UpdateAsync(role);
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

					this.disposed = true;
				}
			}

			base.Dispose(disposing);
		}
	}

	/// <summary>
	/// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>string</c>.
	/// </summary>
	public class RoleStoreMultiHostString : RoleStoreMultiHost<IdentityRoleMultiHostString, string, IdentityUserRoleMultiHostString>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoleStoreMultiHostString" /> class.
		/// </summary>
		/// <param name="context">The <c>DbContext</c>.</param>
		/// <param name="hostId">The host id.</param>
		/// <param name="systemHostId">The system host identifier.</param>
		public RoleStoreMultiHostString(DbContext context, string hostId, string systemHostId)
			: base(context, hostId, systemHostId)
		{
			Contract.Requires<ArgumentNullException>(context != null, "context");
			Contract.Requires<ArgumentNullException>(!hostId.IsNullOrWhiteSpace(), "hostId");
			Contract.Requires<ArgumentNullException>(!systemHostId.IsNullOrWhiteSpace(), "systemHostId");
        }

        /// <summary>
        /// Creates the role manager.
        /// </summary>
        /// <returns></returns>
        protected override RoleManagerMultiHost<IdentityRoleMultiHostString, string, IdentityUserRoleMultiHostString> CreateRoleManager()
        {
            return new RoleManagerMultiHostString(this);
        }

        /// <summary>
        /// Gets a <c>RoleManager</c>.
        /// </summary>
        /// <returns></returns>
        public RoleManagerMultiHostString GetRoleManager()
        {
            return (RoleManagerMultiHostString)base.RoleManager;
        }
	}

	/// <summary>
	/// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>Guid</c>.
	/// </summary>
	public class RoleStoreMultiHostGuid : RoleStoreMultiHost<IdentityRoleMultiHostGuid, Guid, IdentityUserRoleMultiHostGuid>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoleStoreMultiHostGuid" /> class.
		/// </summary>
		/// <param name="context">The <c>DbContext</c>.</param>
		/// <param name="hostId">The host id.</param>
		/// <param name="systemHostId">The system host identifier.</param>
		public RoleStoreMultiHostGuid(DbContext context, Guid hostId, Guid systemHostId)
			: base(context, hostId, systemHostId)
		{
			Contract.Requires<ArgumentNullException>(context != null, "context");
			Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
			Contract.Requires<ArgumentNullException>(systemHostId != Guid.Empty, "systemHostId");
        }

        /// <summary>
        /// Creates the role manager.
        /// </summary>
        /// <returns></returns>
        protected override RoleManagerMultiHost<IdentityRoleMultiHostGuid, Guid, IdentityUserRoleMultiHostGuid> CreateRoleManager()
        {
            return new RoleManagerMultiHostGuid(this);
        }

        /// <summary>
        /// Gets a <c>RoleManager</c>.
        /// </summary>
        /// <returns></returns>
        public RoleManagerMultiHostGuid GetRoleManager()
        {
            return (RoleManagerMultiHostGuid)base.RoleManager;
        }
	}

	/// <summary>
	/// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>int</c>.
	/// </summary>
	public class RoleStoreMultiHostInt : RoleStoreMultiHost<IdentityRoleMultiHostInt, int, IdentityUserRoleMultiHostInt>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoleStoreMultiHostInt" /> class.
		/// </summary>
		/// <param name="context">The <c>DbContext</c>.</param>
		/// <param name="hostId">The host id.</param>
		/// <param name="systemHostId">The system host identifier.</param>
		public RoleStoreMultiHostInt(DbContext context, int hostId, int systemHostId)
			: base(context, hostId, systemHostId)
		{
			Contract.Requires<ArgumentNullException>(context != null, "context");
			Contract.Requires<ArgumentException>(hostId > 0);
			Contract.Requires<ArgumentException>(systemHostId > 0);
        }

        /// <summary>
        /// Creates the role manager.
        /// </summary>
        /// <returns></returns>
        protected override RoleManagerMultiHost<IdentityRoleMultiHostInt, int, IdentityUserRoleMultiHostInt> CreateRoleManager()
        {
            return new RoleManagerMultiHostInt(this);
        }

        /// <summary>
        /// Gets a <c>RoleManager</c>.
        /// </summary>
        /// <returns></returns>
        public RoleManagerMultiHostInt GetRoleManager()
        {
            return (RoleManagerMultiHostInt)base.RoleManager;
        }
	}

	/// <summary>
	/// EntityFramework <c>RoleStore</c> implementation for a multi-tenant <c>DbContext</c> having key and host key data types of <c>long</c>.
	/// </summary>
	public class RoleStoreMultiHostLong : RoleStoreMultiHost<IdentityRoleMultiHostLong, long, IdentityUserRoleMultiHostLong>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoleStoreMultiHostLong"/> class.
		/// </summary>
		/// <param name="context">The <c>DbContext</c>.</param>
		/// <param name="hostId">The host id.</param>
		public RoleStoreMultiHostLong(DbContext context, long hostId, long systemHostId)
			: base(context, hostId, systemHostId)
		{
			Contract.Requires<ArgumentNullException>(context != null, "context");
			Contract.Requires<ArgumentException>(hostId > 0);
			Contract.Requires<ArgumentException>(systemHostId > 0);
		}

        /// <summary>
        /// Creates the role manager.
        /// </summary>
        /// <returns></returns>
        protected override RoleManagerMultiHost<IdentityRoleMultiHostLong, long, IdentityUserRoleMultiHostLong> CreateRoleManager()
        {
            return new RoleManagerMultiHostLong(this);
        }

        /// <summary>
        /// Gets a <c>RoleManager</c>.
        /// </summary>
        /// <returns></returns>
        public RoleManagerMultiHostLong GetRoleManager()
        {
            return (RoleManagerMultiHostLong)base.RoleManager;
        }
	}
}
