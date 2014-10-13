// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE.TXT file for details.
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
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityUserMultiHost<TKey, THostKey> : IdentityUser<TKey, IdentityUserLoginMultiHost<TKey, THostKey>, IdentityUserRoleMultiHost<TKey>, IdentityUserClaimMultiHost<TKey>>, IUserMultiHost<TKey, THostKey>
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
        /// Initializes a new instance of the <see cref="IdentityUserMultiHost{TKey, THostKey}"/> class.
        /// </summary>
        public IdentityUserMultiHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHost{TKey, THostKey}"/> class.
        /// </summary>
        /// <param name="userName">The userName.</param>
        public IdentityUserMultiHost(string userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            UserName = userName;
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>string</c> key type.
    /// </summary>
    public class IdentityUserMultiHostString : IdentityUserMultiHost<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostString"/> class.
        /// </summary>
        public IdentityUserMultiHostString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostString"/> class.
        /// </summary>
        /// <param name="userName">The userName.</param>
        public IdentityUserMultiHostString(string userName) 
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>Guid</c> key type.
    /// </summary>
    public class IdentityUserMultiHostGuid : IdentityUserMultiHost<Guid, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostGuid"/> class.
        /// </summary>
        public IdentityUserMultiHostGuid()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostGuid"/> class.
        /// </summary>
        /// <param name="userName">The userName.</param>
        public IdentityUserMultiHostGuid(string userName)
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>int</c> key type.
    /// </summary>
    public class IdentityUserMultiHostInt : IdentityUserMultiHost<int, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostInt"/> class.
        /// </summary>
        public IdentityUserMultiHostInt()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostInt"/> class.
        /// </summary>
        /// <param name="userName">The user Name.</param>
        public IdentityUserMultiHostInt(string userName)
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>long</c> key type.
    /// </summary>
    public class IdentityUserMultiHostLong : IdentityUserMultiHost<long, long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostLong"/> class.
        /// </summary>
        public IdentityUserMultiHostLong()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHostLong"/> class.
        /// </summary>
        /// <param name="userName">The userName.</param>
        public IdentityUserMultiHostLong(string userName)
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }
}
