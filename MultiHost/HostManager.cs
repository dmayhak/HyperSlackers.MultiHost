using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyperSlackers.AspNet.Identity.EntityFramework.Entities;
using System.Diagnostics.Contracts;
using System.Data.Entity;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    public class HostManager<THost, TKey>
        where THost : IdentityHost<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        public DbContext Context { get; private set; }
        private bool disposed = false;

        public HostManager(HostStore<THost, TKey> store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");

            this.Context = store.Context;
        }

        public virtual async Task CreateAsync(THost host)
        {
            Contract.Requires<ArgumentNullException>(host != null, "host");

            ThrowIfDisposed();

            Context.Set<THost>().Add(host);

            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task DeleteAsync(THost host)
        {
            Contract.Requires<ArgumentNullException>(host != null, "host");

            ThrowIfDisposed();

            Context.Set<THost>().Remove(host);

            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task UpdateAsync(THost host)
        {

            Contract.Requires<ArgumentNullException>(host != null, "host");

            ThrowIfDisposed();

            Context.Entry(host).State = EntityState.Modified;

            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<THost> FindByIdAsync(TKey hostId)
        {
            Contract.Requires<ArgumentNullException>(!hostId.Equals(default(TKey)));

            ThrowIfDisposed();

            return await Context.Set<THost>().FirstOrDefaultAsync(h => h.HostId.Equals(hostId));
        }

        public async Task<THost> FindByNameAsync(string hostName)
        {
            Contract.Requires<ArgumentNullException>(!hostName.IsNullOrWhiteSpace(), "hostName");

            ThrowIfDisposed();

            return await Context.Set<THost>().FirstOrDefaultAsync(h => h.Name.ToUpper() == hostName.ToUpper());
        }

        public async Task<THost> GetSystemHostAsync()
        {
            ThrowIfDisposed();

            return await Context.Set<THost>().FirstOrDefaultAsync(h => h.IsSystemHost);
        }

        public IList<string> GetDomains(TKey hostId)
        {
            ThrowIfDisposed();

            var host = FindByIdAsync(hostId).Result;

            return Context.Set<IdentityHostDomain>().Where(d => d.HostId == host.Id).Select(d => d.DomainName).ToList();
        }

        public virtual async Task AddDomainAsync(THost host, string domainName)
        {
            Contract.Requires<ArgumentNullException>(host != null, "host");
            Contract.Requires<ArgumentNullException>(!domainName.IsNullOrWhiteSpace(), "domainName");

            ThrowIfDisposed();

            // check if domain exists
            if (await Context.Set<IdentityHostDomain>().AnyAsync(d => d.DomainName.ToUpper() == domainName.ToUpper()))
            {
                throw new ArgumentException(string.Format("Domain '{0}' already exists.", domainName));
            }

            Context.Set<IdentityHostDomain>().Add(new IdentityHostDomain() { HostId = host.Id, DomainName = domainName });
        }

        public virtual async Task RemoveDomainAsync(string domainName)
        {
            Contract.Requires<ArgumentNullException>(!domainName.IsNullOrWhiteSpace(), "domainName");

            ThrowIfDisposed();

            // get domain
            var domain = await Context.Set<IdentityHostDomain>().SingleOrDefaultAsync(d => d.DomainName.ToUpper() == domainName.ToUpper());

            if (domain != null)
            {
                Context.Set<IdentityHostDomain>().Remove(domain);
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // TODO: cache references? if so, release them here

                    this.disposed = true;
                }
            }
        }
    }

    public class HostManagerGuid : HostManager<IdentityHostGuid, Guid>
    {
        public HostManagerGuid(HostStoreGuid store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }
    }

    public class HostManagerInt : HostManager<IdentityHostInt, int>
    {
        public HostManagerInt(HostStoreInt store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }
    }

    public class HostManagerLong : HostManager<IdentityHostLong, long>
    {
        public HostManagerLong(HostStoreLong store)
            : base(store)
        {
            Contract.Requires<ArgumentNullException>(store != null, "store");
        }
    }
}
