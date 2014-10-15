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
    /// Entity type for a user's login (i.e. facebook, google) for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityUserLoginMultiHost<TKey, THostKey> : IdentityUserLogin<TKey>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        public virtual THostKey HostId { get; set; }
    }

    /// <summary>
    /// Entity type for a user's login (i.e. facebook, google) for a multi-tenant <c>DbContext</c> having a key type of <c>string</c>.
    /// </summary>
    public class IdentityUserLoginMultiHostString : IdentityUserLoginMultiHost<string, string>
    {
    }

    /// <summary>
    /// Entity type for a user's login (i.e. facebook, google) for a multi-tenant <c>DbContext</c> having a key type of <c>Guid</c>.
    /// </summary>
    public class IdentityUserLoginMultiHostGuid : IdentityUserLoginMultiHost<Guid, Guid>
    {
    }

    /// <summary>
    /// Entity type for a user's login (i.e. facebook, google) for a multi-tenant <c>DbContext</c> having a key type of <c>int</c>.
    /// </summary>
    public class IdentityUserLoginMultiHostInt : IdentityUserLoginMultiHost<int, int>
    {
    }

    /// <summary>
    /// Entity type for a user's login (i.e. facebook, google) for a multi-tenant <c>DbContext</c> having a key type of <c>long</c>.
    /// </summary>
    public class IdentityUserLoginMultiHostLong : IdentityUserLoginMultiHost<long, long>
    {
    }
}
