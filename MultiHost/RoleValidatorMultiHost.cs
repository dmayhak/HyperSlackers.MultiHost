using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    public class RoleValidatorMultiHost<TRole, TKey, TUserRole> : RoleValidator<TRole, TKey>, IDisposable
        where TRole : IdentityRoleMultiHost<TKey, TUserRole>, IRoleMultiHost<TKey>, new()
        where TKey : IEquatable<TKey>
        where TUserRole : IdentityUserRoleMultiHost<TKey>, IUserRoleMultiHost<TKey>, new()
    {
        protected RoleManagerMultiHost<TRole, TKey, TUserRole> Manager { get; private set; }
        private bool disposed = false;

        public RoleValidatorMultiHost(RoleManagerMultiHost<TRole, TKey, TUserRole> manager)
            : base(manager)
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");

            Manager = manager;
        }

        /// <summary>
        /// Validates a role before saving.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public override async Task<IdentityResult> ValidateAsync(TRole role)
        {
            Contract.Requires<ArgumentNullException>(role != null, "role");

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(role.Name))
            {
                errors.Add(String.Format("Role name cannot be empty."));
            }
            else
            {
                // if global role, host id must match; if not, check globals and host
                TRole existing = await Manager.FindByNameAsync(role.HostId, role.Name);
                if (existing != null && !existing.Id.Equals(role.Id))
                {
                    errors.Add(String.Format("Role '{0}' already exists.", role.Name));
                }
            }

            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Manager = null;

                    this.disposed = true;
                }
            }
        }
    }
}
