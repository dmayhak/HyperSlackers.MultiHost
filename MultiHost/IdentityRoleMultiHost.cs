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
    public class IdentityRoleMultiHost<TKey, THostKey> : IdentityRole<TKey, IdentityUserRoleMultiHost<TKey>>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        // this allows for Roles to be different for each host
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
        {
            Contract.Requires<ArgumentException>(!name.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            this.Name = name;
            this.HostId = hostId;
        }
    }

    public class IdentityRoleMultiHostString : IdentityRoleMultiHost<string, string>
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
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace()); 
        }
    }

    public class IdentityRoleMultiHostGuid : IdentityRoleMultiHost<Guid, Guid>
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
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }
    }

    public class IdentityRoleMultiHostInt : IdentityRoleMultiHost<int, int>
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
            Contract.Requires<ArgumentNullException>(hostId > 0, "hostId");
        }
    }

    public class IdentityRoleMultiHostLong : IdentityRoleMultiHost<long, long>
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
            Contract.Requires<ArgumentNullException>(hostId > 0, "hostId");
        }
    }
}
