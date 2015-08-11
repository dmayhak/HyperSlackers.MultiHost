using HyperSlackers.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MultiHostDemo.Entities;

namespace ClassroomForOne.Extensions
{
    public static class UserDbSetExtensions0
    {
        /// <summary>
        /// Returns true if the specified email address is already in use by
        /// either the current host, or by a global user.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static bool EmailExists(this IDbSet<AppUser> set, string email)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");

            return set.Repo().UserManager.FindByEmail(email) != null;
        }

        /// <summary>
        /// Returns true if the specified email address is already in use by
        /// either the specified host, or by a global user.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static bool EmailExists(this IDbSet<AppUser> set, Guid hostId, string email)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");

            return set.Repo().UserManager.FindByEmail(hostId, email) != null;
        }

        /// <summary>
        /// Returns true if the specified name address is already in use by
        /// either the current host, or by a global user.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static bool NameExists(this IDbSet<AppUser> set, string name)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");

            return set.Repo().UserManager.FindByName(name) != null;
        }

        /// <summary>
        /// Returns true if the specified name address is already in use by
        /// either the specified host, or by a global user.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static bool NameExists(this IDbSet<AppUser> set, Guid hostId, string name)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");

            return set.Repo().UserManager.FindByName(hostId, name) != null;
        }

        /// <summary>
        /// Creates the specified user without a password.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="user">The user.</param>
        public static void Create(this IDbSet<AppUser> set, AppUser user)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(user != null, "user");

            set.Repo().UserManager.Create(user);
        }

        /// <summary>
        /// Creates the specified user with the given password.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        public static void Create(this IDbSet<AppUser> set, AppUser user, string password)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(!password.IsNullOrWhiteSpace(), "password");

            set.Repo().UserManager.Create(user, password);
        }

        /// <summary>
        /// Finds a user with the specified name for the current host (or global).
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static AppUser FindByName(this IDbSet<AppUser> set, string name)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");

            return set.Repo().UserManager.FindByName(name);
        }

        /// <summary>
        /// Finds a user with the specified name for the specified host (or global).
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static AppUser FindByName(this IDbSet<AppUser> set, Guid hostId, string name)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
            Contract.Requires<ArgumentNullException>(!name.IsNullOrWhiteSpace(), "name");

            return set.Repo().UserManager.FindByName(hostId, name);
        }

        /// <summary>
        /// Finds a user with the specified email for the current host (or global).
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static AppUser FindByEmail(this IDbSet<AppUser> set, string email)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");

            return set.Repo().UserManager.FindByEmail(email);
        }

        /// <summary>
        /// Finds a user with the specified email for the specified host (or global).
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static AppUser FindByEmail(this IDbSet<AppUser> set, Guid hostId, string email)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");

            return set.Repo().UserManager.FindByEmail(hostId, email);
        }

        /// <summary>
        /// Adds the user to role.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        public static void AddUserToRole(this IDbSet<AppUser> set, Guid userId, RoleType role)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(userId != Guid.Empty, "userId");

            set.Repo().UserManager.AddToRole(userId, role.ToString());
        }

        /// <summary>
        /// REturns true if the user is in the given role.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public static bool UserHasRole(this IDbSet<AppUser> set, Guid userId, RoleType role)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(userId != Guid.Empty, "userId");

            return set.Repo().UserManager.IsInRole(userId, role.ToString());
        }

        /// <summary>
        /// Removes the user from role.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        public static void RemoveUserFromRole(this IDbSet<AppUser> set, Guid userId, RoleType role)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(userId != Guid.Empty, "userId");

            set.Repo().UserManager.RemoveFromRole(userId, role.ToString());
        }

        /// <summary>
        /// Finds a user with the specified email and password for the specified host (or global).
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static AppUser FindByEmailAndPassword(this IDbSet<AppUser> set, Guid hostId, string email, string password)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");
            Contract.Requires<ArgumentNullException>(!password.IsNullOrWhiteSpace(), "password");

            return set.Repo().UserManager.Find(hostId, email, password);
        }

        /// <summary>
        /// Finds a user with the specified email and password for the current host (or global).
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static AppUser FindByEmailAndPassword(this IDbSet<AppUser> set, string email, string password)
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");
            Contract.Requires<ArgumentNullException>(!email.IsNullOrWhiteSpace(), "email");
            Contract.Requires<ArgumentNullException>(!password.IsNullOrWhiteSpace(), "password");

            return set.Repo().UserManager.Find(email, password);
        }
    }
}
