using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c>.
    /// </summary>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityUserRoleMultiHost<TKey> : IdentityUserRole<TKey>, IUserRoleMultiHost<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey HostId { get; set; }
        public bool IsGlobal { get; set; }
    }

    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c> having a key type of <c>Guid</c>.
    /// </summary>
    public class IdentityUserRoleMultiHostGuid : IdentityUserRoleMultiHost<Guid>, IUserRoleMultiHostGuid
    {
    }

    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c> having a key type of <c>int</c>.
    /// </summary>
    public class IdentityUserRoleMultiHostInt : IdentityUserRoleMultiHost<int>, IUserRoleMultiHostInt
    {
    }

    /// <summary>
    /// Entity type that represents a user belonging to a role in a multi-tenant <c>DbContext</c> having a key type of <c>long</c>.
    /// </summary>
    public class IdentityUserRoleMultiHostLong : IdentityUserRoleMultiHost<long>, IUserRoleMultiHostLong
    {
    }
}
