using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Minimal interface for a user in a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    /// <typeparam name="TKey">The host id key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public interface IUserMultiHost<TKey> : IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey HostId { get; set; }
        bool IsGlobal { get; set; }
    }

    /// <summary>
    /// Minimal interface for a user in a multi-tenant <c>DbContext</c> having key types of <c>Guid</c>.
    /// </summary>
    public interface IUserMultiHostGuid : IUserMultiHost<Guid>
    {
    }

    /// <summary>
    /// Minimal interface for a user in a multi-tenant <c>DbContext</c> having key types of <c>int</c>.
    /// </summary>
    public interface IUserMultiHostInt : IUserMultiHost<int>
    {
    }

    /// <summary>
    /// Minimal interface for a user in a multi-tenant <c>DbContext</c> having key types of <c>long</c>.
    /// </summary>
    public interface IUserMultiHostLong : IUserMultiHost<long>
    {
    }
}
