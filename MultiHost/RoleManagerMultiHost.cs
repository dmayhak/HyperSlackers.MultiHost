﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    public class RoleManagerMultiHost<TKey, THostKey, TRole> : RoleManager<TRole, TKey>
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
        where TRole : IdentityRoleMultiHost<TKey, THostKey, IdentityUserRoleMultiHost<TKey>>, new()
    {
        public virtual THostKey HostId { get; set; } // TODO: do we need this?   are roles specific to each host?

        public RoleManagerMultiHost(RoleStoreMultiHost<TKey, THostKey, TRole> store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        public RoleManagerMultiHost(RoleStoreMultiHost<TKey, THostKey, TRole> store, THostKey hostId)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            this.HostId = hostId;
        }

        public override async Task<bool> RoleExistsAsync(string roleName)
        {
            //Contract.Requires<ArgumentException>(!roleName.IsNullOrWhiteSpace());

            return await RoleExistsAsync(roleName, this.HostId);
        }

        public async Task<bool> RoleExistsAsync(string roleName, THostKey hostId) // not really async...
        {
            Contract.Requires<ArgumentException>(!roleName.IsNullOrWhiteSpace());
            Contract.Requires<ArgumentNullException>(!EqualityComparer<THostKey>.Default.Equals(hostId, default(THostKey)), "hostId");

            return Roles.Any(r => r.Name == roleName && r.HostId.Equals(hostId));
        }
    }

    public class RoleManagerMultiHostString : RoleManagerMultiHost<string, string, IdentityRoleMultiHost<string, string, IdentityUserRoleMultiHost<string>>>
    {
        public RoleManagerMultiHostString(RoleStoreMultiHostString store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        public RoleManagerMultiHostString(RoleStoreMultiHostString store, string hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentException>(!hostId.IsNullOrWhiteSpace());
        }
    }

    public class RoleManagerMultiHostGuid : RoleManagerMultiHost<Guid, Guid, IdentityRoleMultiHost<Guid, Guid, IdentityUserRoleMultiHost<Guid>>>
    {
        public RoleManagerMultiHostGuid(RoleStoreMultiHostGuid store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        public RoleManagerMultiHostGuid(RoleStoreMultiHostGuid store, Guid hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
        }
    }

    public class RoleManagerMultiHostInt : RoleManagerMultiHost<int, int, IdentityRoleMultiHost<int, int, IdentityUserRoleMultiHost<int>>>
    {
        public RoleManagerMultiHostInt(RoleStoreMultiHostInt store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        public RoleManagerMultiHostInt(RoleStoreMultiHostInt store, int hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentException>(hostId > 0, "hostId");
        }
    }

    public class RoleManagerMultiHostLong : RoleManagerMultiHost<long, long, IdentityRoleMultiHost<long, long, IdentityUserRoleMultiHost<long>>>
    {
        public RoleManagerMultiHostLong(RoleStoreMultiHostLong store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }

        public RoleManagerMultiHostLong(RoleStoreMultiHostLong store, long hostId)
            : base(store, hostId)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
            Contract.Requires<ArgumentException>(hostId > 0, "hostId");
        }
    }
}