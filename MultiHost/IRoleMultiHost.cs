// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Minimal interface for a role for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public interface IRoleMultiHost<TKey> : IRole<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey HostId { get; set; }
        bool IsGlobal { get; set; }
    }

    /// <summary>
    /// Minimal interface for a role for a multi-tenant <c>DbContext</c> having key type of <c>string</c>.
    /// </summary>
    public interface IRoleMultiHostString : IRoleMultiHost<string>
    {
    }

    /// <summary>
    /// Minimal interface for a role for a multi-tenant <c>DbContext</c> having key type of <c>Guid</c>.
    /// </summary>
    public interface IRoleMultiHostGuid : IRoleMultiHost<Guid>
    {
    }

    /// <summary>
    /// Minimal interface for a role for a multi-tenant <c>DbContext</c> having key type of <c>int</c>.
    /// </summary>
    public interface IRoleMultiHostInt : IRoleMultiHost<int>
    {
    }

    /// <summary>
    /// Minimal interface for a role for a multi-tenant <c>DbContext</c> having key type of <c>long</c>.
    /// </summary>
    public interface IRoleMultiHostLong : IRoleMultiHost<long>
    {
    }
}
