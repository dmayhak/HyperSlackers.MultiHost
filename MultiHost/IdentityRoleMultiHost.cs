// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Represents a <c>Role</c> entity for a multi-tenant DbContext.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityRoleMultiHost<TKey, TUserRole> : IdentityRole<TKey, TUserRole>, IRoleMultiHost<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<TKey>, IUserRoleMultiHost<TKey>, new()
    {
        public TKey HostId { get; set; }
        public bool IsGlobal { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHost{TKey}"/> class.
        /// </summary>
        public IdentityRoleMultiHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHost{TKey}"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        public IdentityRoleMultiHost(string name)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");

            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHost{TKey}"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="hostId">The host id.</param>
        public IdentityRoleMultiHost(string name, TKey hostId)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");

            this.Name = name;
            this.HostId = hostId;
        }
    }

    /// <summary>
    /// Multi-tenant <c>Role</c> having both key and host key as <c>Guid</c> types
    /// </summary>
    public class IdentityRoleMultiHostGuid : IdentityRoleMultiHost<Guid, IdentityUserRoleMultiHostGuid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostGuid"/> class.
        /// </summary>
        public IdentityRoleMultiHostGuid()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostGuid"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public IdentityRoleMultiHostGuid(string name)
            : base(name)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostGuid"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hostId">The host identifier.</param>
        public IdentityRoleMultiHostGuid(string name, Guid hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }
    }

    /// <summary>
    /// Multi-tenant <c>Role</c> having both key and host key as <c>int</c> types
    /// </summary>
    public class IdentityRoleMultiHostInt : IdentityRoleMultiHost<int, IdentityUserRoleMultiHostInt>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostInt"/> class.
        /// </summary>
        public IdentityRoleMultiHostInt()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostInt"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public IdentityRoleMultiHostInt(string name)
            : base(name)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostInt"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hostId">The host identifier.</param>
        public IdentityRoleMultiHostInt(string name, int hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");
            Contract.Requires<ArgumentException>(hostId > 0);
        }
    }

    /// <summary>
    /// Multi-tenant <c>Role</c> having both key and host key as <c>long</c> types
    /// </summary>
    public class IdentityRoleMultiHostLong : IdentityRoleMultiHost<long, IdentityUserRoleMultiHostLong>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostLong"/> class.
        /// </summary>
        public IdentityRoleMultiHostLong()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostLong"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public IdentityRoleMultiHostLong(string name)
            : base(name)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostLong"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hostId">The host identifier.</param>
        public IdentityRoleMultiHostLong(string name, long hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");
            Contract.Requires<ArgumentException>(hostId > 0);
        }
    }
}
