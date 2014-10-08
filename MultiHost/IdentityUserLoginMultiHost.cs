using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    public class IdentityUserLoginMultiHost<TKey, THostKey> : IdentityUserLogin<TKey>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        public virtual THostKey HostId { get; set; }
    }

    public class IdentityUserLoginMultiHostString : IdentityUserLoginMultiHost<string, string>
    {
    }

    public class IdentityUserLoginMultiHostGuid : IdentityUserLoginMultiHost<Guid, Guid>
    {
    }

    public class IdentityUserLoginMultiHostInt : IdentityUserLoginMultiHost<int, int>
    {
    }

    public class IdentityUserLoginMultiHostLong : IdentityUserLoginMultiHost<long, long>
    {
    }
}
