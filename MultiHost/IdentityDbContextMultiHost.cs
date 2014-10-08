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
    public class IdentityDbContextMultiHost<TUser, TRole, TKey, THostKey, TUserLogin, TUserRole, TUserClaim> : IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TUser : IdentityUserMultiHost<TKey, THostKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRoleMultiHost<TKey, THostKey, TUserRole>
        where TUserLogin : IdentityUserLoginMultiHost<TKey, THostKey>, new()
        where TUserRole : IdentityUserRoleMultiHost<TKey>, new()
        where TUserClaim : IdentityUserClaimMultiHost<TKey>, new()
        where TKey : IEquatable<TKey>
        where THostKey : IEquatable<THostKey>
    {
        public IdentityDbContextMultiHost()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContextMultiHost(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Contract.Requires<ArgumentException>(!nameOrConnectionString.IsNullOrWhiteSpace());
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUserLogin>().HasKey(e => new { e.HostId, e.LoginProvider, e.ProviderKey, e.UserId });

            // this has to be in derived types because of the string version of HostId
            //x modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
            modelBuilder.Entity<TUser>().Property(u => u.UserName).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 1) { IsUnique = true }));
            modelBuilder.Entity<TUserLogin>().HasKey(ul => new { ul.HostId, ul.LoginProvider, ul.ProviderKey, ul.UserId });
        }
    }

    public class IdentityDbContextMultiHostString<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostString, string, string, IdentityUserLoginMultiHostString, IdentityUserRoleMultiHostString, IdentityUserClaimMultiHostString>
        where TUser : IdentityUserMultiHostString
    {
        public IdentityDbContextMultiHostString()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContextMultiHostString(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Contract.Requires<ArgumentException>(!nameOrConnectionString.IsNullOrWhiteSpace());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }

    public class IdentityDbContextMultiHostGuid<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostGuid, Guid, Guid, IdentityUserLoginMultiHostGuid, IdentityUserRoleMultiHostGuid, IdentityUserClaimMultiHostGuid>
        where TUser : IdentityUserMultiHostGuid
    {
        public IdentityDbContextMultiHostGuid()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContextMultiHostGuid(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Contract.Requires<ArgumentException>(!nameOrConnectionString.IsNullOrWhiteSpace());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }

    public class IdentityDbContextMultiHostInt<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostInt, int, int, IdentityUserLoginMultiHostInt, IdentityUserRoleMultiHostInt, IdentityUserClaimMultiHostInt>
        where TUser : IdentityUserMultiHostInt
    {
        public IdentityDbContextMultiHostInt()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContextMultiHostInt(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Contract.Requires<ArgumentException>(!nameOrConnectionString.IsNullOrWhiteSpace());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }

    public class IdentityDbContexMultiHosttLong<TUser> : IdentityDbContextMultiHost<TUser, IdentityRoleMultiHostLong, long, long, IdentityUserLoginMultiHostLong, IdentityUserRoleMultiHostLong, IdentityUserClaimMultiHostLong>
        where TUser : IdentityUserMultiHostLong
    {
        public IdentityDbContexMultiHosttLong()
            : this("DefaultConnection")
        {
        }

        public IdentityDbContexMultiHosttLong(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Contract.Requires<ArgumentException>(!nameOrConnectionString.IsNullOrWhiteSpace());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().Property(u => u.HostId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex", 0) { IsUnique = true }));
        }
    }
}
