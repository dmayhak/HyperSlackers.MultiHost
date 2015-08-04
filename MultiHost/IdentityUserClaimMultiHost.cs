using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
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
