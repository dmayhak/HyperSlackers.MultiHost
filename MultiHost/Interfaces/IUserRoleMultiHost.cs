﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Minimal interface for a user role for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type for the database. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public interface IUserRoleMultiHost<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey HostId { get; set; }
        bool IsGlobal { get; set; }
    }

    /// <summary>
    /// Minimal interface for a user role for a multi-tenant <c>DbContext</c> having key type of <c>Guid</c>.
    /// </summary>
    public interface IUserRoleMultiHostGuid : IUserRoleMultiHost<Guid>
    {
    }

    /// <summary>
    /// Minimal interface for a user role for a multi-tenant <c>DbContext</c> having key type of <c>int</c>.
    /// </summary>
    public interface IUserRoleMultiHostInt : IUserRoleMultiHost<int>
    {
    }

    /// <summary>
    /// Minimal interface for a user role for a multi-tenant <c>DbContext</c> having key type of <c>long</c>.
    /// </summary>
    public interface IUserRoleMultiHostLong : IUserRoleMultiHost<long>
    {
    }
}
