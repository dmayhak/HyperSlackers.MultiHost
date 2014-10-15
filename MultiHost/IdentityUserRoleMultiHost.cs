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
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityUserRoleMultiHost<TKey> : IdentityUserRole<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c> having a key type of <c>string</c>.
    /// </summary>
    public class IdentityUserRoleMultiHostString : IdentityUserRoleMultiHost<string>
    {
    }

    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c> having a key type of <c>Guid</c>.
    /// </summary>
    public class IdentityUserRoleMultiHostGuid : IdentityUserRoleMultiHost<Guid>
    {
    }

    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c> having a key type of <c>int</c>.
    /// </summary>
    public class IdentityUserRoleMultiHostInt : IdentityUserRoleMultiHost<int>
    {
    }

    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c> having a key type of <c>long</c>.
    /// </summary>
    public class IdentityUserRoleMultiHostLong : IdentityUserRoleMultiHost<long>
    {
    }
}
