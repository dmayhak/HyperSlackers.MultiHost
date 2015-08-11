using HyperSlackers.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using MultiHostDemo.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiHostDemo.Repository
{
    public class ApplicationDbContextBase : IdentityDbContextMultiHostGuid<AppUser>
    {
        private static bool initializerSet = SetInitializer();

        // table renaming; override to have custom table names for the Identity Tables
        public override string SchemaName { get { return "Common"; } }
        public override string UsersTableName { get { return "Users"; } }
        public override string RolesTableName { get { return "Roles"; } }
        public override string UserClaimsTableName { get { return "UserClaims"; } }
        public override string UserLoginsTableName { get { return "UserLogins"; } }
        public override string UserRolesTableName { get { return "UserRoles"; } }
        public override string HostsTableName { get { return "Hosts"; } }
        public override string HostDomainsTableName { get { return "HostDomains"; } }

        public override string AuditSchemaName { get { return "Auditing"; } }
        public override string AuditsTableName { get { return "Audits"; } }
        public override string AuditItemsTableName { get { return "AuditItems"; } }
        public override string AuditPropertiesTableName { get { return "AuditProperties"; } }


        private UserStoreMultiHostGuid<AppUser> userStore;
        private UserManagerMultiHostGuid<AppUser> userManager;
        private RoleStoreMultiHostGuid roleStore;
        private RoleManagerMultiHostGuid roleManager;
        private HostStoreGuid hostStore;
        private HostManagerGuid hostManager;

        protected ApplicationDbContextBase(Guid systemHostId)
            : this("DefaultConnection", systemHostId)
        {
        }

        protected ApplicationDbContextBase(string nameOrConnectionString, Guid systemHostId)
            : base(nameOrConnectionString, systemHostId)
        {
        }

        private static bool SetInitializer()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, MigrationConfiguration>());

            return true;
        }

        /// <summary> This method is called when the model for a derived context has been initialized, but
        ///           before the model has been locked down and used to initialize the context.  The
        ///           default implementation of this method does nothing, but it can be overridden in a
        ///           derived class such that the model can be further configured before it is locked
        ///           down. </summary>
        ///
        /// <param name="modelBuilder"> The builder that defines the model for the context being created. </param>
        ///
        /// <seealso cref="M:System.Data.Entity.DbContext.OnModelCreating(DbModelBuilder)"/>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public UserStoreMultiHostGuid<AppUser> UserStore
        {
            get
            {
                if (userStore == null)
                {
                    userStore = new UserStoreMultiHostGuid<AppUser>(this, this.SystemHostId, this.HostId);
                }

                return userStore;
            }
        }

        public RoleStoreMultiHostGuid RoleStore
        {
            get
            {
                if (roleStore == null)
                {
                    roleStore = new RoleStoreMultiHostGuid(this, this.SystemHostId, this.HostId);
                }

                return roleStore;
            }
        }

        public RoleManagerMultiHostGuid RoleManager
        {
            get
            {
                if (roleManager == null)
                {
                    roleManager = new RoleManagerMultiHostGuid(RoleStore);
                }

                return roleManager;
            }
        }

        public UserManagerMultiHostGuid<AppUser> UserManager
        {
            get
            {
                if (userManager == null)
                {
                    userManager = new UserManagerMultiHostGuid<AppUser>(UserStore);

                    // base implementation has AllowOnlyAlphanumericUserNames = false and RequireUniqueEmail = false
                    userManager.PasswordValidator = new PasswordValidator() { RequiredLength = 2 };
                }

                return userManager;
            }
        }

        public HostManagerGuid HostManager
        {
            get
            {
                if (hostManager == null)
                {
                    hostManager = new HostManagerGuid(HostStore);
                }

                return hostManager;
            }
        }

        public HostStoreGuid HostStore
        {
            get
            {
                if (hostStore == null)
                {
                    hostStore = new HostStoreGuid(this);
                }

                return hostStore;
            }
        }
    }
}
