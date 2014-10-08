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
    public class IdentityUserMultiHost<TKey, THostKey, TLogin, TRole, TClaim> : IdentityUser<TKey, TLogin, TRole, TClaim>, IUserMultiHost<TKey, THostKey>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
        where TLogin : IdentityUserLoginMultiHost<TKey, THostKey>
        where TRole : IdentityUserRoleMultiHost<TKey>
        where TClaim : IdentityUserClaimMultiHost<TKey>
    {
        public THostKey HostId { get; set; }
    }

    public class IdentityUserMultiHostString :  IdentityUserMultiHost<string, string, IdentityUserLoginMultiHostString, IdentityUserRoleMultiHostString, IdentityUserClaimMultiHostString>
    {
        public IdentityUserMultiHostString()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityUserMultiHostString(string userName) 
            : this()
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            UserName = userName;
        }
    }

    public class IdentityUserMultiHostGuid : IdentityUserMultiHost<Guid, Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid>
    {
        public IdentityUserMultiHostGuid()
        {
            Id = Guid.NewGuid();
        }

        public IdentityUserMultiHostGuid(string userName) 
            : this()
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            UserName = userName;
        }
    }

    public class IdentityUserMultiHostInt : IdentityUserMultiHost<int, int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt>
    {
        public IdentityUserMultiHostInt()
        {
            Id = 0; // TODO: how to assign this?
        }

        public IdentityUserMultiHostInt(string userName) 
            : this()
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            UserName = userName;
        }
    }

    public class IdentityUserMultiHostLong : IdentityUserMultiHost<long, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong>
    {
        public IdentityUserMultiHostLong()
        {
            Id = 0; // TODO: how to assign this?
        }

        public IdentityUserMultiHostLong(string userName)
            : this()
        {
            Contract.Requires<ArgumentException>(!userName.IsNullOrWhiteSpace());

            UserName = userName;
        }
    }
}
