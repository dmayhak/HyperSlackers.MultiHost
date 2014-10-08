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
    public class IdentityRoleMultiHost<TKey, THostKey, TUserRole> : IdentityRole<TKey, TUserRole>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
        where TUserRole : IdentityUserRoleMultiHost<TKey>
    {
        // this allows for Roles to be different for each host
        // TODO: use default(THostKey) for global Roles???
        public THostKey HostId { get; set; } 

        public IdentityRoleMultiHost()
        {
        }

        public IdentityRoleMultiHost(string name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());

            this.Name = name;
        }

        public IdentityRoleMultiHost(string name, THostKey hostId)
            : this(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            this.HostId = hostId;
        }
    }

    public class IdentityRoleMultiHostString : IdentityRoleMultiHost<string, string, IdentityUserRoleMultiHostString>
    {
        public IdentityRoleMultiHostString()
        {
        }

        public IdentityRoleMultiHostString(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        public IdentityRoleMultiHostString(string name, string hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace()); // what about string.Empty for site-wide roles?
        }
    }

    public class IdentityRoleMultiHostGuid : IdentityRoleMultiHost<Guid, Guid, IdentityUserRoleMultiHostGuid>
    {
        public IdentityRoleMultiHostGuid()
        {
        }

        public IdentityRoleMultiHostGuid(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        public IdentityRoleMultiHostGuid(string name, Guid hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId"); // what about Guid.Empty for site-wide roles?
        }
    }

    public class IdentityRoleMultiHostInt : IdentityRoleMultiHost<int, int, IdentityUserRoleMultiHostInt>
    {
        public IdentityRoleMultiHostInt()
        {
        }

        public IdentityRoleMultiHostInt(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        public IdentityRoleMultiHostInt(string name, int hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(hostId > 0, "hostId"); // what about 0 for site-wide roles?
        }
    }

    public class IdentityRoleMultiHostLong : IdentityRoleMultiHost<long, long, IdentityUserRoleMultiHostLong>
    {
        public IdentityRoleMultiHostLong()
        {
        }

        public IdentityRoleMultiHostLong(string name)
            : base(name)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
        }

        public IdentityRoleMultiHostLong(string name, long hostId)
            : base(name, hostId)
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(hostId > 0, "hostId"); // what about 0 for site-wide roles?
        }
    }
}
