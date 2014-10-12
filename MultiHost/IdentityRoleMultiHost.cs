using Microsoft.AspNet.Identity.EntityFramework;
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
    /// Represents a <c>Role</c> entity for a multi-tenant DbContext.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityRoleMultiHost<TKey, THostKey> : IdentityRole<TKey, IdentityUserRoleMultiHost<TKey>>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        /// <summary>
        /// Gets or sets the host id.
        /// </summary>
        /// <value>
        /// The host id.
        /// </value>
        public THostKey HostId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHost{TKey, THostKey}"/> class.
        /// </summary>
        public IdentityRoleMultiHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHost{TKey, THostKey}"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        public IdentityRoleMultiHost(string name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());

            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHost{TKey, THostKey}"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="hostId">The host ide.</param>
        public IdentityRoleMultiHost(string name, THostKey hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            this.Name = name;
            this.HostId = hostId;
        }
    }

    /// <summary>
    /// Multi-tenant <c>Role</c> having both key and host key as <c>string</c> types
    /// </summary>
    public class IdentityRoleMultiHostString : IdentityRoleMultiHost<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostString"/> class.
        /// </summary>
        public IdentityRoleMultiHostString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostString"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        public IdentityRoleMultiHostString(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostString"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="hostId">The host id.</param>
        public IdentityRoleMultiHostString(string name, string hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace()); 
        }
    }

    /// <summary>
    /// Multi-tenant <c>Role</c> having both key and host key as <c>Guid</c> types
    /// </summary>
    public class IdentityRoleMultiHostGuid : IdentityRoleMultiHost<Guid, Guid>
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
        /// <param name="name">The role name.</param>
        public IdentityRoleMultiHostGuid(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostGuid"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="hostId">The host id.</param>
        public IdentityRoleMultiHostGuid(string name, Guid hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }
    }

    /// <summary>
    /// Multi-tenant <c>Role</c> having both key and host key as <c>int</c> types
    /// </summary>
    public class IdentityRoleMultiHostInt : IdentityRoleMultiHost<int, int>
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
        /// <param name="name">The role name.</param>
        public IdentityRoleMultiHostInt(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostInt"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="hostId">The host id.</param>
        public IdentityRoleMultiHostInt(string name, int hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(hostId > 0, "hostId");
        }
    }

    /// <summary>
    /// Multi-tenant <c>Role</c> having both key and host key as <c>long</c> types
    /// </summary>
    public class IdentityRoleMultiHostLong : IdentityRoleMultiHost<long, long>
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
        /// <param name="name">The role name.</param>
        public IdentityRoleMultiHostLong(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleMultiHostLong"/> class.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="hostId">The host id.</param>
        public IdentityRoleMultiHostLong(string name, long hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(hostId > 0, "hostId");
        }
    }
}
