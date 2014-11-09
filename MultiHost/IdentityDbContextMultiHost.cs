// Copyright (C) 2014 Del Mayhak
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using Microsoft.AspNet.Identity;
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
using HyperSlackers.AspNet.Identity.EntityFramework.Entities;
using System.Reflection;
using System.Data.Entity.Core.Objects;
using System.Web;
using System.Collections;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Generic <c>IdentityDbContextMultiHost</c> base that allows the ASP.NET Identity system to support multiple
    /// hosts in a single application (multi-tenancy).
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityRoleMultiHost{TKey}</c>.</typeparam>
    /// <typeparam name="TKey">The key type. (Typically <c>string</c>, <c>Guid</c>, <c>int</c>, or <c>long</c>.)</typeparam>
    public class IdentityDbContextMultiHost<THost, TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim, TAudit, TAuditItem> : AuditingDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim, TAudit, TAuditItem> // IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where THost : IdentityHost<TKey>, new()
        where TUser : IdentityUserMultiHost<TKey, TUserLogin, TUserRole, TUserClaim>, IUserMultiHost<TKey>, new()
        where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLoginMultiHost<TKey>, IUserLoginMultiHost<TKey>, new()
        where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
        where TUserClaim : IdentityUserClaimMultiHost<TKey>, IUserClaimMultiHost<TKey>, new()
        where TAudit : Audit<TKey>, new()
        where TAuditItem : AuditItem<TKey>, new()
    {
        public TKey SystemHostId { get; private set; }

        // table renaming; override to have custom table names
        public virtual string SchemaName { get { return ""; } }
        public virtual string UsersTableName { get { return "AspNetUsers"; } }
        public virtual string RolesTableName { get { return "AspNetRoles"; } }
        public virtual string UserClaimsTableName { get { return "AspNetUserClaims"; } }
        public virtual string UserLoginsTableName { get { return "AspNetUserLogins"; } }
        public virtual string UserRolesTableName { get { return "AspNetUserRoles"; } }
        public virtual string HostsTableName { get { return "AspNetHosts"; } }
        public virtual string HostDomainsTableName { get { return "AspNetHostDomains"; } }

        // host dbsets
        public DbSet<THost> Hosts { get; set; }
        public DbSet<IdentityHostDomain> HostDomains { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHost{TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim}" /> class.
        /// </summary>
        public IdentityDbContextMultiHost(TKey systemHostId)
            : this("DefaultConnection", systemHostId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHost{TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim}" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHost(string nameOrConnectionString, TKey systemHostId)
            : base(nameOrConnectionString)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");

            this.SystemHostId = systemHostId;
        }

        protected override TKey GetHostId()
        {
            var hostName = GetHostName();
            var hostDomain = this.Set<IdentityHostDomain>().SingleOrDefault(d => d.DomainName.ToUpper() == hostName.ToUpper());

            if (hostDomain != null)
            {
                var host = this.Set<THost>().SingleOrDefault(h => h.Id == hostDomain.HostId);
                if (host != null)
                {
                    return host.HostId;
                }
            }

            // if not found, return system host id as default
            return SystemHostId;
        }

        protected override TKey GetUserId()
        {
            var hostId = GetHostId();
            var userName = GetUserName();

            if (!hostId.Equals(default(TKey)) && !userName.IsNullOrWhiteSpace() && userName != "<system>")
            {
                var user = this.Set<TUser>().SingleOrDefault(u => u.UserName.ToUpper() == userName.ToUpper() && (u.HostId.Equals(hostId) || u.IsGlobal == true));
                if (user != null)
                {
                    return user.Id;
                }
            }

            // TODO: windows, or just go to base class?

            return base.GetUserId();
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
                    if (Users.Any(u => u.UserName.ToUpper() == user.UserName.ToUpper() && u.HostId.Equals(user.HostId)))
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
            modelBuilder.Entity<TRole>()
                .ToTable((RolesTableName.IsNullOrWhiteSpace() ? "AspNetRoles" : RolesTableName), (SchemaName.IsNullOrWhiteSpace() ? "dbo" : SchemaName));
            modelBuilder.Entity<TUserClaim>()
                .ToTable((UserClaimsTableName.IsNullOrWhiteSpace() ? "AspNetUserClaims" : UserClaimsTableName), (SchemaName.IsNullOrWhiteSpace() ? "dbo" : SchemaName));
            modelBuilder.Entity<TUserLogin>()
                .ToTable((UserLoginsTableName.IsNullOrWhiteSpace() ? "AspNetUserLogins" : UserLoginsTableName), (SchemaName.IsNullOrWhiteSpace() ? "dbo" : SchemaName))
                .HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId });
            modelBuilder.Entity<TUserRole>()
                .ToTable((UserRolesTableName.IsNullOrWhiteSpace() ? "AspNetUserRoles" : UserRolesTableName), (SchemaName.IsNullOrWhiteSpace() ? "dbo" : SchemaName))
                .HasKey(e => new { e.UserId, e.RoleId });
            modelBuilder.Entity<TUser>()
                .ToTable((UsersTableName.IsNullOrWhiteSpace() ? "AspNetUsers" : UsersTableName), (SchemaName.IsNullOrWhiteSpace() ? "dbo" : SchemaName));
            modelBuilder.Entity<TUser>()
                .Property(u => u.UserName).IsRequired()
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 1) { IsUnique = true }));

           // hosts tables
            modelBuilder.Entity<THost>()
                .ToTable((HostsTableName.IsNullOrWhiteSpace() ? "AspNetHosts" : HostsTableName), (SchemaName.IsNullOrWhiteSpace() ? "dbo" : SchemaName));
            modelBuilder.Entity<IdentityHostDomain>()
                 .ToTable((HostDomainsTableName.IsNullOrWhiteSpace() ? "AspNetHostDomains" : HostDomainsTableName), (SchemaName.IsNullOrWhiteSpace() ? "dbo" : SchemaName));

        }

        //protected virtual TKey GetCurrentUserId()
        //{
        //    if (this.UserId.Equals(default(TKey)))
        //    {
        //        System.Web.HttpContext context = System.Web.HttpContext.Current;
        //        if (context != null)
        //        {
        //            if (context.User != null)
        //            {
        //                if (context.User.Identity != null)
        //                {
        //                    var userName = context.User.Identity.Name;
        //                    var user = this.Users.Where(u => u.UserName == userName && )
        //                }
        //            }
        //        }
        //    }

        //    return this.UserId;
        //}

        //protected virtual TKey GetCurrentHostId()
        //{
        //    return default(TKey); // TODO:
        //}
    }

    /// <summary>
    /// Multi-tenant <c>IdentityDContest</c> having both key and host key as <c>Guid</c> types
    /// </summary>
    /// <typeparam name="TUser">A user type derived from <c>IdentityUserMultiHostGuid</c>.</typeparam>
    public class IdentityDbContextMultiHostGuid<TUser> : IdentityDbContextMultiHost<IdentityHostGuid, TUser, IdentityRoleMultiHostGuid, Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid, AuditGuid, AuditItemGuid>
        where TUser : IdentityUserMultiHostGuid, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostGuid{TUser}"/> class.
        /// </summary>
        public IdentityDbContextMultiHostGuid(Guid systemHostId)
            : this("DefaultConnection", systemHostId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostGuid{TUser}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHostGuid(string nameOrConnectionString, Guid systemHostId)
            : base(nameOrConnectionString, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
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
    public class IdentityDbContextMultiHostInt<TUser> : IdentityDbContextMultiHost<IdentityHostInt, TUser, IdentityRoleMultiHostInt, int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt, AuditInt, AuditItemInt>
        where TUser : IdentityUserMultiHostInt, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostInt{TUser}"/> class.
        /// </summary>
        public IdentityDbContextMultiHostInt(int systemHostId)
            : this("DefaultConnection", systemHostId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContextMultiHostInt{TUser}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHostInt(string nameOrConnectionString, int systemHostId)
            : base(nameOrConnectionString, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
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
    public class IdentityDbContextMultiHostLong<TUser> : IdentityDbContextMultiHost<IdentityHostLong, TUser, IdentityRoleMultiHostLong, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong, AuditLong, AuditItemLong>
        where TUser : IdentityUserMultiHostLong, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContexMultiHosttLong{TUser}"/> class.
        /// </summary>
        public IdentityDbContextMultiHostLong(long systemHostId)
            : this("DefaultConnection", systemHostId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContexMultiHosttLong{TUser}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public IdentityDbContextMultiHostLong(string nameOrConnectionString, long systemHostId)
            : base(nameOrConnectionString, systemHostId)
        {
            Contract.Requires<ArgumentNullException>(!nameOrConnectionString.IsNullOrWhiteSpace(), "nameOrConnectionString");
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

