using Microsoft.AspNet.Identity;
// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    public static class RoleManagerExtensions
    {
        /// <summary>
        ///     Create a role
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IdentityResult Create<TRole, TKey, TUserRole>(this RoleManagerMultiHost<TRole, TKey, TUserRole> manager, string roleName, bool global = false)
            where TKey : IEquatable<TKey>
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            var hostId = global ? manager.SystemHostId : manager.HostId;

            return AsyncHelper.RunSync(() => manager.CreateAsync(hostId, roleName, global));
        }
        /// <summary>
        ///     Create a role
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IdentityResult Create<TRole, TKey, TUserRole>(this RoleManagerMultiHost<TRole, TKey, TUserRole> manager, TKey hostId, string roleName, bool global = false)
            where TKey : IEquatable<TKey>
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            return AsyncHelper.RunSync(() => manager.CreateAsync(hostId, roleName, global));
        }

        /// <summary>
        /// Find a role by name
        /// </summary>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public static TRole FindByName<TRole, TKey, TUserRole>(this RoleManagerMultiHost<TRole, TKey, TUserRole> manager, TKey hostId, string roleName)
            where TKey : IEquatable<TKey>
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            return AsyncHelper.RunSync(() => manager.FindByNameAsync(hostId, roleName));
        }

        /// <summary>
        /// Returns true if the role exists
        /// </summary>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">manager</exception>
        public static bool RoleExists<TRole, TKey, TUserRole>(this RoleManagerMultiHost<TRole, TKey, TUserRole> manager, TKey hostId, string roleName)
            where TKey : IEquatable<TKey>
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            return AsyncHelper.RunSync(() => manager.RoleExistsAsync(hostId, roleName));
        }
    }
}
