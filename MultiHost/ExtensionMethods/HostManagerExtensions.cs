using HyperSlackers.AspNet.Identity.EntityFramework.Entities;
// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    public static class HostManagerExtensions
    {
        public static void Create<THost, TKey>(this HostManager<THost, TKey> manager, THost host)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(host != null, "host");

            AsyncHelper.RunSync(() => manager.CreateAsync(host));
        }

        public static void Delete<THost, TKey>(this HostManager<THost, TKey> manager, THost host)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(host != null, "host");

            AsyncHelper.RunSync(() => manager.DeleteAsync(host));
        }

        public static void Update<THost, TKey>(this HostManager<THost, TKey> manager, THost host)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(host != null, "host");

            AsyncHelper.RunSync(() => manager.UpdateAsync(host));
        }

        public static THost FindById<THost, TKey>(this HostManager<THost, TKey> manager, TKey hostId)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)));

            return AsyncHelper.RunSync(() => manager.FindByIdAsync(hostId));
        }

        public static THost FindByName<THost, TKey>(this HostManager<THost, TKey> manager, string hostName)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostName.IsNullOrWhiteSpace(), "hostName");

            return AsyncHelper.RunSync(() => manager.FindByNameAsync(hostName));
        }

        public static THost GetSystemHost<THost, TKey>(this HostManager<THost, TKey> manager)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            return AsyncHelper.RunSync(() => manager.GetSystemHostAsync());
        }

        public static void AddDomain<THost, TKey>(this HostManager<THost, TKey> manager, THost host, string domainName)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(host != null, "host");
            Contract.Requires<ArgumentNullException>(!domainName.IsNullOrWhiteSpace(), "domainName");

            AsyncHelper.RunSync(() => manager.AddDomainAsync(host, domainName));
        }

        public static void RemoveDomain<THost, TKey>(this HostManager<THost, TKey> manager, string domainName)
            where THost : IdentityHost<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!domainName.IsNullOrWhiteSpace(), "domainName");

            AsyncHelper.RunSync(() => manager.RemoveDomainAsync(domainName));
        }
    }
}
