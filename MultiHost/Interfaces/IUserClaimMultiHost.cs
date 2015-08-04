using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Minimal interface for a user claim for a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public interface IUserClaimMultiHost<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey HostId { get; set; }
        bool IsGlobal { get; set; }
    }

    /// <summary>
    /// Minimal interface for a user claim for a multi-tenant <c>DbContext</c> having key type of <c>Guid</c>.
    /// </summary>
    public interface IUserClaimMultiHostGuid : IUserClaimMultiHost<Guid>
    {
    }

    /// <summary>
    /// Minimal interface for a user claim for a multi-tenant <c>DbContext</c> having key type of <c>int</c>.
    /// </summary>
    public interface IUserClaimMultiHostInt : IUserClaimMultiHost<int>
    {
    }

    /// <summary>
    /// Minimal interface for a user claim for a multi-tenant <c>DbContext</c> having key type of <c>long</c>.
    /// </summary>
    public interface IUserClaimMultiHostLong : IUserClaimMultiHost<long>
    {
    }
}
