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

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Minimal interface for a user login for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The database key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public interface IUserLoginMultiHost<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey HostId { get; set; }
        bool IsGlobal { get; set; }
    }

    /// <summary>
    /// Minimal interface for a user login for a multi-tenant <c>DbContext</c> having key type of <c>Guid</c>.
    /// </summary>
    public interface IUserLoginMultiHostGuid : IUserLoginMultiHost<Guid>
    {
    }

    /// <summary>
    /// Minimal interface for a user login for a multi-tenant <c>DbContext</c> having key type of <c>int</c>.
    /// </summary>
    public interface IUserLoginMultiHostInt : IUserLoginMultiHost<int>
    {
    }

    /// <summary>
    /// Minimal interface for a user login for a multi-tenant <c>DbContext</c> having key type of <c>long</c>.
    /// </summary>
    public interface IUserLoginMultiHostLong : IUserLoginMultiHost<long>
    {
    }
}
