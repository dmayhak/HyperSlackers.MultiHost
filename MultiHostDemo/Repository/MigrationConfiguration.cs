using HyperSlackers.AspNet.Identity.EntityFramework;
using HyperSlackers.AspNet.Identity.EntityFramework.Entities;
using MultiHostDemo.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiHostDemo.Repository
{
    internal sealed class MigrationConfiguration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            //AutomaticMigrationsEnabled = true; //! can cause issues with some index schemes--most notably clustered index changes
            //AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // add the default host
            var host = context.Hosts.SingleOrDefault(h => h.HostId == context.SystemHostId);
            if (host == null)
            {
                context.Hosts.Add(new IdentityHostGuid() { HostId = context.SystemHostId, Name = "<system host>", IsSystemHost = true });
                //context.SaveChanges();
            }

            // add the roles
            foreach (var roleType in Enum.GetValues(typeof(RoleType)))
            {
                var role = context.Roles.SingleOrDefault(r => r.Name == roleType.ToString());
                if (role == null)
                {
                    context.Roles.Add(new IdentityRoleMultiHostGuid() { Id = Guid.NewGuid(), HostId = context.SystemHostId, Name = roleType.ToString(), IsGlobal = true });
                    //context.SaveChanges();
                }
            }

            // add second host
            var hostId = new Guid("d572b5be-8116-4ef0-b2ea-51667658a8f5");
            host = context.Hosts.SingleOrDefault(h => h.HostId == hostId);
            if (host == null)
            {
                context.Hosts.Add(new IdentityHostGuid() { HostId = hostId, Name = "localhost", IsSystemHost = false });
                //context.SaveChanges();
            }

            // add super user account
            var suId = new Guid("bf784c72-c224-47d0-9ca6-2f719d46adbc");
            var su = context.Users.SingleOrDefault(u => u.Id == suId);
            if (su == null)
            {
                context.Users.Add(new AppUser()
                {
                    Id = suId,
                    Email = "super@user.com",
                    FirstName = "Super",
                    LastName = "User",
                    FullName = "Super User",
                    DisplayName = "The Man!",
                    UserName = "super@user.com"
                });
                //context.SaveChanges();
            }

            // add an admin user account for localhost
            var adminId = new Guid("e6ed778f-9ee0-4fde-987f-e42e24939078");
            var admin = context.Users.SingleOrDefault(u => u.Id == adminId);
            if (admin == null)
            {
                context.Users.Add(new AppUser()
                {
                    Id = adminId,
                    Email = "admin@localhost.com",
                    FirstName = "Admin",
                    LastName = "Localhost",
                    FullName = "Admin Localhost",
                    DisplayName = "LHAdmin",
                    UserName = "jim.oleary@exammatrix.com"
                });
                //context.SaveChanges();
            }

            context.SaveChanges();

            // HACK: the below code should only run once...

            host = context.Hosts.SingleOrDefault(h => h.HostId == hostId);
            var hostManager = context.HostManager;
            hostManager.AddDomain(host, "localhost"); // coming to site via localhost will cause host to be this one;
                                                        // coming to site via 127.0.0.1 will cause host to be the system host
                                                        // play with adding additional hosts and using the hosts file to test

            // add users to roles (if they are already in the role, the add will be ignored
            var userManager = context.UserManager;
            userManager.AddToRole(context.SystemHostId, suId, RoleType.Super.ToString()); // role granted at the system host
            userManager.AddToRole(hostId, adminId, RoleType.Admin.ToString()); // role granted to "localhost" host

            userManager.AddPasswordAsync(suId, "Password");
            userManager.AddPasswordAsync(adminId, "Pwd");
        }
    }
}
