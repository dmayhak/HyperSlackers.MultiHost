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
    public class ApplicationDbContext : ApplicationDbContextBase
    {
        public static readonly Guid SystemClientId = new Guid("a678c417-865b-4f19-b079-7a471e48d9ec");

        public ApplicationDbContext()
            : this("DefaultConnectionString")
        {
        }

        public ApplicationDbContext(string connectionString)
            : base(connectionString, SystemClientId)
        {
        }


        // Users and Roles handled in base classes


    }
}
