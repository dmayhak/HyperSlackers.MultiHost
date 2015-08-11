using HyperSlackers.AspNet.Identity.EntityFramework;
using MultiHostDemo.Repository;
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

namespace ClassroomForOne.Extensions
{
    public static class IDbSetExtensions0
    {
        internal static ApplicationDbContext Repo<T>(this IDbSet<T> set) where T : class
        {
            Contract.Requires<ArgumentNullException>(set != null, "set");

            return set.GetContext() as ApplicationDbContext;
        }

        internal static DbContext GetContext<T>(this IDbSet<T> dbSet) where T : class
        {
            Contract.Requires<ArgumentNullException>(dbSet != null, "dbSet");
            object internalSet = dbSet.GetType().GetField("_internalSet", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dbSet);
            object internalContext = internalSet.GetType().BaseType.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(internalSet);

            return (DbContext)internalContext.GetType().GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public).GetValue(internalContext, null);
        }

        //public static T[] AddIfNotExists<T>(this IDbSet<T> set, params T[] entities) where T : class, IEntity
        //{
        //    Contract.Requires<ArgumentNullException>(set != null, "set");
        //    Contract.Requires<ArgumentNullException>(entities != null, "entities");

        //    List<T> foundOrAdded = new List<T>();

        //    foreach (var item in entities)
        //    {
        //        foundOrAdded.Add(set.AddIfNotExists(item));
        //    }

        //    return foundOrAdded.ToArray();
        //}

        //public static T AddIfNotExists<T>(this IDbSet<T> set, T entity) where T : class, IEntity
        //{
        //    Contract.Requires<ArgumentNullException>(set != null, "set");
        //    Contract.Requires<ArgumentNullException>(entity != null, "entity");

        //    if (!set.Exists(entity.EntityId))
        //    {
        //        return set.Add(entity);
        //    }
        //    else
        //    {
        //        return set.Find(entity.EntityId);
        //    }
        //}

        //public static bool Exists<T>(this IDbSet<T> set, Guid id) where T : class, IEntity
        //{
        //    Contract.Requires<ArgumentNullException>(set != null, "set");

        //    return set.Find(id) != null;
        //}

        //public static T FindOrDefault<T>(this IDbSet<T> set, Guid id) where T : class, IEntity
        //{
        //    Contract.Requires<ArgumentNullException>(set != null, "set");

        //    return set.Find(id);
        //}

        //public static T FindOrNew<T>(this IDbSet<T> set, Guid id) where T : class, IEntity
        //{
        //    Contract.Requires<ArgumentNullException>(set != null, "set");
        //    Contract.Ensures(Contract.Result<T>() != null);

        //    var entity = set.Find(id);

        //    if (entity == null)
        //    {
        //        entity = (T)Activator.CreateInstance(typeof(T));
        //        entity.EntityId = Guid.NewGuid();
        //        set.Add(entity);
        //    }

        //    return entity;
        //}

        //public static T FirstOrNew<T>(this IDbSet<T> set, Expression<Func<T, bool>> predicate) where T : class, IEntity
        //{
        //    Contract.Requires<ArgumentNullException>(set != null, "set");

        //    T entity = set.FirstOrDefault(predicate);

        //    if (entity == null)
        //    {
        //        entity = set.New();
        //    }

        //    return entity;
        //}

        //public static T New<T>(this IDbSet<T> set) where T : class, IEntity
        //{
        //    Contract.Requires<ArgumentNullException>(set != null, "set");
        //    Contract.Ensures(Contract.Result<T>() != null);

        //    T entity = (T)Activator.CreateInstance(typeof(T));
        //    if (entity.EntityId == Guid.Empty)
        //    {
        //        entity.EntityId = Guid.NewGuid();
        //    }

        //    set.Add(entity);

        //    return entity;
        //}
    }
}
