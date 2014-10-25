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
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="TLogin">The type of the login.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <typeparam name="TClaim">The type of the claim.</typeparam>
    public class IdentityUserMultiHost<TKey, TLogin, TRole, TClaim> : IdentityUser<TKey, TLogin, TRole, TClaim>, IUserMultiHost<TKey>
        where TKey : IEquatable<TKey>
        where TLogin : Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<TKey>, IUserLoginMultiHost<TKey>, new()
        where TRole : Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<TKey>, IUserRoleMultiHost<TKey>, new()
        where TClaim : Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<TKey>, IUserClaimMultiHost<TKey>, new()
    {
        public TKey HostId { get; set; }
        public bool IsGlobal { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHost{TKey}"/> class.
        /// </summary>
        public IdentityUserMultiHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserMultiHost{TKey}"/> class.
        /// </summary>
        /// <param name="userName">The userName.</param>
        public IdentityUserMultiHost(string userName)
        {
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");

            UserName = userName;
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>string</c> key type.
    /// </summary>
    public class IdentityUserMultiHostString : IdentityUserMultiHost<string, IdentityUserLoginMultiHostString, IdentityUserRoleMultiHostString, IdentityUserClaimMultiHostString>, IUserMultiHostString
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
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>Guid</c> key type.
    /// </summary>
    public class IdentityUserMultiHostGuid : IdentityUserMultiHost<Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid>, IUserMultiHostGuid
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
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>int</c> key type.
    /// </summary>
    public class IdentityUserMultiHostInt : IdentityUserMultiHost<int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt>, IUserMultiHostInt
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
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");
        }
    }

    /// <summary>
    /// EntityFramework <c>IdentityUser</c> implementation for a multi-tenant <c>DbContext</c> having a <c>long</c> key type.
    /// </summary>
    public class IdentityUserMultiHostLong : IdentityUserMultiHost<long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong>, IUserMultiHostLong
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
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");
        }
    }
}
