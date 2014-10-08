using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.MultiHost
{
    public class IdentityUserRoleMultiHost<TKey> : IdentityUserRole<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    public class IdentityUserRoleMultiHostString : IdentityUserRoleMultiHost<string>
    {
    }

    public class IdentityUserRoleMultiHostGuid : IdentityUserRoleMultiHost<Guid>
    {
    }

    public class IdentityUserRoleMultiHostInt : IdentityUserRoleMultiHost<int>
    {
    }

    public class IdentityUserRoleMultiHostLong : IdentityUserRoleMultiHost<long>
    {
    }
}
