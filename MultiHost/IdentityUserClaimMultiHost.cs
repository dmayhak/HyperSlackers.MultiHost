// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Entity type that represents one specific user claim in a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityUserClaimMultiHost<TKey> : IdentityUserClaim<TKey>, IUserClaimMultiHost<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey HostId { get; set; }
        public bool IsGlobal { get; set; }
    }

    /// <summary>
    /// User claim entity type having a key type of <c>string</c>.
    /// </summary>
    public class IdentityUserClaimMultiHostString : IdentityUserClaimMultiHost<string>, IUserClaimMultiHostString
    {
    }

    /// <summary>
    /// User claim entity type having a key type of <c>Guid</c>.
    /// </summary>
    public class IdentityUserClaimMultiHostGuid : IdentityUserClaimMultiHost<Guid>, IUserClaimMultiHostGuid
    {
    }

    /// <summary>
    /// User claim entity type having a key type of <c>int</c>.
    /// </summary>
    public class IdentityUserClaimMultiHostInt : IdentityUserClaimMultiHost<int>, IUserClaimMultiHostInt
    {
    }

    /// <summary>
    /// User claim entity type having a key type of <c>long</c>.
    /// </summary>
    public class IdentityUserClaimMultiHostLong : IdentityUserClaimMultiHost<long>, IUserClaimMultiHostLong
    {
    }
}
