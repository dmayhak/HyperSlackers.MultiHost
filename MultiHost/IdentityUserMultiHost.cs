using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    public class IdentityUserMultiHost<TKey, THostKey> : IdentityUser<TKey, IdentityUserLoginMultiHost<TKey, THostKey>, IdentityUserRoleMultiHost<TKey>, IdentityUserClaimMultiHost<TKey>>, IUserMultiHost<TKey, THostKey>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        public THostKey HostId { get; set; }

        public IdentityUserMultiHost()
        {
        }

        public IdentityUserMultiHost(string userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            UserName = userName;
        }
    }

    public class IdentityUserMultiHostString : IdentityUserMultiHost<string, string>
    {
        public IdentityUserMultiHostString()
        {
        }

        public IdentityUserMultiHostString(string userName) 
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }

    public class IdentityUserMultiHostGuid : IdentityUserMultiHost<Guid, Guid>
    {
        public IdentityUserMultiHostGuid()
        {
        }

        public IdentityUserMultiHostGuid(string userName)
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }

    public class IdentityUserMultiHostInt : IdentityUserMultiHost<int, int>
    {
        public IdentityUserMultiHostInt()
        {
        }

        public IdentityUserMultiHostInt(string userName)
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }

    public class IdentityUserMultiHostLong : IdentityUserMultiHost<long, long>
    {
        public IdentityUserMultiHostLong()
        {
        }

        public IdentityUserMultiHostLong(string userName)
            : base(userName)
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());
        }
    }
}
