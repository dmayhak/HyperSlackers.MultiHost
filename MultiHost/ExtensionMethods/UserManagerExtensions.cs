using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Return a user with the specified username and password or null if there is no match.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static TUser Find<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, string userName, string password)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");
            Contract.Requires<ArgumentNullException>(!password.IsNullOrWhiteSpace(), "password");

            return AsyncHelper.RunSync(() => manager.FindAsync(hostId, userName, password));
        }

        /// <summary>
        /// Find a user by name
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static TUser FindByName<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, string userName)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userName.IsNullOrWhiteSpace(), "userName");

            return AsyncHelper.RunSync(() => manager.FindByNameAsync(hostId, userName));
        }

        /// <summary>
        /// Find a user by email
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static TUser FindByEmail<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, string email)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");

            return AsyncHelper.RunSync(() => manager.FindByEmailAsync(hostId, email));
        }

        /// <summary>
        /// Sync extension
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public static TUser Find<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, UserLoginInfo login)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(login != null, "login");

            return AsyncHelper.RunSync(() => manager.FindAsync(hostId, login));
        }

        /// <summary>
        /// Add a user claim
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public static IdentityResult AddClaim<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, TKey userId, Claim claim)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            return AsyncHelper.RunSync(() => manager.AddClaimAsync(hostId, userId, claim));
        }

        /// <summary>
        /// Remove a user claim
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public static IdentityResult RemoveClaim<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, TKey userId, Claim claim)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            return AsyncHelper.RunSync(() => manager.RemoveClaimAsync(hostId, userId, claim));
        }

        /// <summary>
        /// Get a users's claims
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static IList<Claim> GetClaims<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, TKey userId)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");

            return AsyncHelper.RunSync(() => manager.GetClaimsAsync(hostId, userId));
        }

        /// <summary>
        /// Add a user to a role
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public static IdentityResult AddToRole<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, TKey userId, string roleName)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, IUserMultiHost<TKey>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            return AsyncHelper.RunSync(() => manager.AddToRoleAsync(hostId, userId, roleName));
        }

        /// <summary>
        /// Remove a user from a role.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public static IdentityResult RemoveFromRole<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, TKey userId, string roleName)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            return AsyncHelper.RunSync(() => manager.RemoveFromRoleAsync(hostId, userId, roleName));
        }

        /// <summary>
        /// Get a users's roles
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static IList<string> GetRoles<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, TKey userId)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");

            return AsyncHelper.RunSync(() => manager.GetRolesAsync(hostId, userId));
        }

        /// <summary>
        /// Returns true if the user is in the specified role
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TUserLogin">The type of the user login.</typeparam>
        /// <typeparam name="TUserRole">The type of the user role.</typeparam>
        /// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">manager</exception>
        public static bool IsInRole<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>(this UserManagerMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> manager, TKey hostId, TKey userId, string roleName)
            where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, new()
            where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
            where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
            where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!userId.Equals(default(TKey)), "userId");
            Contract.Requires<ArgumentNullException>(!roleName.IsNullOrWhiteSpace(), "roleName");

            return AsyncHelper.RunSync(() => manager.IsInRoleAsync(userId, roleName));
        }
    }
}
