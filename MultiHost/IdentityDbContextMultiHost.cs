// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSlackers.MultiHost.Extensions;

namespace HyperSlackers.MultiHost
{
    /// <summary>
    /// Generic <c>IdentityDbContextMultiHost</c> base that allows the ASP.NET Identity system to support multiple
    /// hosts in a single application (multi-tenancy).
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityRoleMultiHost{TKey}</c>.</typeparam>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityDbContextMultiHost<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TKey : IEquatable<TKey>
        where TUser : Microsoft.AspNet.Identity.EntityFramework.IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>, IUserMultiHost<TKey>, new()
        where TRole : Microsoft.AspNet.Identity.EntityFramework.IdentityRole<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
        where TUserLogin : Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<TKey>, IUserLoginMultiHost<TKey>, new()
        where TUserRole : Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<TKey>, IUserRoleMultiHost<TKey>, new()
        where TUserClaim : Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<TKey>, IUserClaimMultiHost<TKey>, new()
    {
        public TKey HostId { get; private set; }
        public TKey SystemHostId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHost{TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim}"/> class.
        /// </summary>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="systemHostId">The system host identifier.</param>
        public IdentityDbContextMultiHost(TKey hostId, TKey systemHostId)
            : this("DefaultConnection", hostId, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!systemHostId.Equals(default(TKey)), "systemHostId");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHost{TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        /// <param name="hostId">The host identifier.</param>
        /// <param name="systemHostId">The system host identifier.</param>
        public IdentityDbContextMultiHost(string nameOrConnectionString, TKey hostId, TKey systemHostId)
            : base(nameOrConnectionString)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)), "hostId");
            Contract.Requires<ArgumentNullException>(!systemHostId.Equals(default(TKey)), "systemHostId");
        }

        /// <summary>
        /// Validates that UserNames are unique per host and case insensitive.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            // TODO: validate the IsGlobal flags for entities that have it (i.e. must belong to system host)
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                var user = entityEntry.Entity as TUser;
                if (user != null)
                {
                    // TODO: test that this is what we really want here....
                    if (Users.Any(u => u.UserName == user.UserName && u.HostId.Equals(user.HostId)))
                    {
                        return new DbEntityValidationResult(entityEntry, new[] { new DbValidationError("UserName", "UserName already exists for Host") });
                    }

                    return new DbEntityValidationResult(entityEntry, Enumerable.Empty<DbValidationError>());
                }
            }

            return base.ValidateEntity(entityEntry, items);
        }

        /// <summary>
        /// Maps table names, and sets up relationships between the various user entities.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TUser HostId has to be in derived types...
            modelBuilder.Entity<TRole>().ToTable("AspNetRoles");
            modelBuilder.Entity<TUserClaim>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<TUserLogin>().ToTable("AspNetUserLogins").HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId });
            modelBuilder.Entity<TUserRole>().ToTable("AspNetUserRoles").HasKey(e => new { e.UserId, e.RoleId });
            modelBuilder.Entity<TUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<TUser>().Property(u => u.UserName).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 1) { IsUnique = true }));
        }
    }

    /// <summary>
    /// Multi-tenant <c>IdentityDContest</c> having key as <c>string</c> types
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHostString</c>.</typeparam>
    public class IdentityDbContextMultiHostString<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostString, string, IdentityUserLoginMultiHostString, IdentityUserRoleMultiHostString, IdentityUserClaimMultiHostString>
        where TUser : IdentityUserMultiHostString, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostString{TUser}"/> class.
        /// </summary>
        public IdentityDbContextMultiHostString(string hostId, string systemHostId)
            : this("DefaultConnection", hostId, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!hostId.IsNullOrWhiteSpace(), "hostId");
            Contract.Requires<ArgumentNullException>(!systemHostId.IsNullOrWhiteSpace(), "systemHostId");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostString{TUser}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHostString(string nameOrConnectionString, string hostId, string systemHostId)
            : base(nameOrConnectionString, hostId, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
            Contract.Requires<ArgumentNullException>(!hostId.IsNullOrWhiteSpace(), "hostId");
            Contract.Requires<ArgumentNullException>(!systemHostId.IsNullOrWhiteSpace(), "systemHostId");
        }

        /// <summary>
        /// Maps table names, and sets up relationships between the various user entities.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }

    /// <summary>
    /// Multi-tenant <c>IdentityDContest</c> having both key and host key as <c>Guid</c> types
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHostGuid</c>.</typeparam>
    public class IdentityDbContextMultiHostGuid<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostGuid, Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid>
        where TUser : IdentityUserMultiHostGuid, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostGuid{TUser}"/> class.
        /// </summary>
        public IdentityDbContextMultiHostGuid(Guid hostId, Guid systemHostId)
            : this("DefaultConnection", hostId, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
            Contract.Requires<ArgumentNullException>(systemHostId != Guid.Empty, "systemHostId");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostGuid{TUser}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHostGuid(string nameOrConnectionString, Guid hostId, Guid systemHostId)
            : base(nameOrConnectionString, hostId, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
            Contract.Requires<ArgumentNullException>(hostId != Guid.Empty, "hostId");
            Contract.Requires<ArgumentNullException>(systemHostId != Guid.Empty, "systemHostId");
        }

        /// <summary>
        /// Maps table names, and sets up relationships between the various user entities.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }

    /// <summary>
    /// Multi-tenant <c>IdentityDContest</c> having both key and host key as <c>int</c> types
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHostInt</c>.</typeparam>
    public class IdentityDbContextMultiHostInt<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostInt, int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt>
        where TUser : IdentityUserMultiHostInt, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostInt{TUser}"/> class.
        /// </summary>
        public IdentityDbContextMultiHostInt(int hostId, int systemHostId)
            : this("DefaultConnection", hostId, systemHostId)
        {
            Contract.Requires<ArgumentException>(hostId > 0);
            Contract.Requires<ArgumentException>(systemHostId > 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostInt{TUser}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHostInt(string nameOrConnectionString, int hostId, int systemHostId)
            : base(nameOrConnectionString, hostId, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
            Contract.Requires<ArgumentException>(hostId > 0);
            Contract.Requires<ArgumentException>(systemHostId > 0);
        }

        /// <summary>
        /// Maps table names, and sets up relationships between the various user entities.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }

    /// <summary>
    /// Multi-tenant <c>IdentityDContest</c> having both key and host key as <c>long</c> types
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHostLong</c>.</typeparam>
    public class IdentityDbContextMultiHostLong<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostLong, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong>
    //public class IdentityDbContexMultiHosttLong<TUser> : IdentityDbContext<TUser, IdentityRoleMultiHostLong, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong>
        where TUser : IdentityUserMultiHostLong, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContexMultiHosttLong{TUser}"/> class.
        /// </summary>
        public IdentityDbContextMultiHostLong(long hostId, long systemHostId)
            : this("DefaultConnection", hostId, systemHostId)
        {
            Contract.Requires<ArgumentException>(hostId > 0);
            Contract.Requires<ArgumentException>(systemHostId > 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContexMultiHosttLong{TUser}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHostLong(string nameOrConnectionString, long hostId, long systemHostId)
            : base(nameOrConnectionString, hostId, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
            Contract.Requires<ArgumentException>(hostId > 0);
            Contract.Requires<ArgumentException>(systemHostId > 0);
        }

        /// <summary>
        /// Maps table names, and sets up relationships between the various user entities.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }
}
