using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    public class IdentityUserClaimMultiHost<TKey> : IdentityUserClaim<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    public class IdentityUserClaimMultiHostString : IdentityUserClaimMultiHost<string>
    {
    }

    public class IdentityUserClaimMultiHostGuid : IdentityUserClaimMultiHost<Guid>
    {
    }

    public class IdentityUserClaimMultiHostInt : IdentityUserClaimMultiHost<int>
    {
    }

    public class IdentityUserClaimMultiHostLong : IdentityUserClaimMultiHost<long>
    {
    }
}
