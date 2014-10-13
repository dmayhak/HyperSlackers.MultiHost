// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE.TXT file for details.
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Minimal interface for a user with id and username for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="THostKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public interface IUserMultiHost<out TKey, THostKey> : IUser<TKey>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        THostKey HostId { get; set; }
    }

    /// <summary>
    /// Minimal interface for a user with id and username for a multi-tenant <c>DbContext</c> having key and host key types of <c>string</c>.
    /// </summary>
    public interface IUserMultiHostString : IUserMultiHost<string, string>
    {
    }

    /// <summary>
    /// Minimal interface for a user with id and username for a multi-tenant <c>DbContext</c> having key and host key types of <c>Guid</c>.
    /// </summary>
    public interface IUserMultiHostGuid : IUserMultiHost<Guid, Guid>
    {
    }

    /// <summary>
    /// Minimal interface for a user with id and username for a multi-tenant <c>DbContext</c> having key and host key types of <c>int</c>.
    /// </summary>
    public interface IUserMultiHostInt : IUserMultiHost<int, int>
    {
    }

    /// <summary>
    /// Minimal interface for a user with id and username for a multi-tenant <c>DbContext</c> having key and host key types of <c>long</c>.
    /// </summary>
    public interface IUserMultiHostLong : IUserMultiHost<long, long>
    {
    }
}
