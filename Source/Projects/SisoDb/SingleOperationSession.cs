using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.EnsureThat;
using SisoDb.Querying;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class SingleOperationSession : ISingleOperationSession
    {
        protected readonly ISisoDatabase Db;

        public SingleOperationSession(ISisoDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();
            Db = db;
        }

        public virtual ISisoQueryable<T> Query<T>() where T : class
        {
            return new SisoReadOnceQueryable<T>(
                Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas), 
                () => Db.BeginSession());
        }

        public virtual TId[] GetIds<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetIds<T, TId>(predicate).ToArray();
            }
        }

        public virtual async Task<TId[]> GetIdsAsync<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return (await session.GetIdsAsync<T, TId>(predicate)).ToArray();
            }
        }

        public virtual object[] GetIds<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetIds(predicate).ToArray();
            }
        }

        public virtual async Task<object[]> GetIdsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return (await session.GetIdsAsync(predicate)).ToArray();
            }
        }

        public virtual T GetByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetByQuery(predicate);
            }
        }

        public virtual async Task<T> GetByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return await session.GetByQueryAsync(predicate);
            }
        }

        public virtual T GetById<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetById<T>(id);
            }
        }

        public virtual async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return await session.GetByIdAsync<T>(id);
            }
        }

        public virtual object GetById(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetById(structureType, id);
            }
        }

        public virtual async Task<object> GetByIdAsync(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return await session.GetByIdAsync(structureType, id);
            }
        }

        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session = Db.BeginSession())
			{
				return session.GetByIdAs<TContract, TOut>(id);
			}
		}

        public virtual async Task<TOut> GetByIdAsAsync<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return await session.GetByIdAsAsync<TContract, TOut>(id);
            }
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
    	{
			Ensure.That(id, "id").IsNotNull();

    		using (var session = Db.BeginSession())
    		{
    			return session.GetByIdAsJson<T>(id);
    		}
    	}

        public virtual async Task<string> GetByIdAsJsonAsync<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return await session.GetByIdAsJsonAsync<T>(id);
            }
        }

        public virtual string GetByIdAsJson(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetByIdAsJson(structureType, id);
            }
        }

        public virtual async Task<string> GetByIdAsJsonAsync(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return await session.GetByIdAsJsonAsync(structureType, id);
            }
        }

        public virtual T[] GetByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session = Db.BeginSession())
			{
				return session.GetByIds<T>(ids).ToArray();
			}
		}

        public virtual async Task<T[]> GetByIdsAsync<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return (await session.GetByIdsAsync<T>(ids)).ToArray();
            }
        }

        public virtual object[] GetByIds(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return session.GetByIds(structureType, ids).ToArray();
            }
        }

        public virtual async Task<object[]> GetByIdsAsync(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return (await session.GetByIdsAsync(structureType, ids)).ToArray();
            }
        }

        public virtual TOut[] GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session = Db.BeginSession())
			{
				return session.GetByIdsAs<TContract, TOut>(ids).ToArray();
			}
		}

        public virtual async Task<TOut[]> GetByIdsAsAsync<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return (await session.GetByIdsAsAsync<TContract, TOut>(ids)).ToArray();
            }
        }

        public virtual string[] GetByIdsAsJson<T>(params object[] ids) where T : class
    	{
    		Ensure.That(ids, "ids").HasItems();

    		using (var session = Db.BeginSession())
    		{
    			return session.GetByIdsAsJson<T>(ids).ToArray();
    		}
    	}

        public virtual async Task<string[]> GetByIdsAsJsonAsync<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return (await session.GetByIdsAsJsonAsync<T>(ids)).ToArray();
            }
        }

        public virtual string[] GetByIdsAsJson(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return session.GetByIdsAsJson(structureType, ids).ToArray();
            }
        }

        public virtual async Task<string[]> GetByIdsAsJsonAsync(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return (await session.GetByIdsAsJsonAsync(structureType, ids)).ToArray();
            }
        }

        public virtual void Insert<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Insert(item);
            }
        }

        public virtual async Task InsertAsync<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertAsync(item);
            }
        }

        public virtual void Insert(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Insert(structureType, item);
            }
        }

        public virtual async Task InsertAsync(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertAsync(structureType, item);
            }
        }

        public virtual void InsertAs<T>(object item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertAs<T>(item);
            }
        }

        public virtual async Task InsertAsAsync<T>(object item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertAsAsync<T>(item);
            }
        }

        public virtual void InsertAs(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using(var session = Db.BeginSession())
            {
                session.InsertAs(structureType, item);
            }
        }

        public virtual async Task InsertAsAsync(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertAsAsync(structureType, item);
            }
        }

        public virtual string InsertJson<T>(string json) where T : class
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = Db.BeginSession())
            {
                return session.InsertJson<T>(json);
            }
        }

        public virtual async Task<string> InsertJsonAsync<T>(string json) where T : class
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = Db.BeginSession())
            {
                return await session.InsertJsonAsync<T>(json);
            }
        }

        public virtual string InsertJson(Type structureType, string json)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = Db.BeginSession())
            {
                return session.InsertJson(structureType, json);
            }
        }

        public virtual async Task<string> InsertJsonAsync(Type structureType, string json)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = Db.BeginSession())
            {
                return await session.InsertJsonAsync(structureType, json);
            }
        }

        public virtual void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Ensure.That(items, "items").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertMany(items);
            }
        }

        public virtual async Task InsertManyAsync<T>(IEnumerable<T> items) where T : class
        {
            Ensure.That(items, "items").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertManyAsync(items);
            }
        }

        public virtual void InsertMany(Type structureType, IEnumerable<object> items)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(items, "items").IsNotNull();

            using(var session = Db.BeginSession())
            {
                session.InsertMany(structureType, items);
            }
        }

        public virtual async Task InsertManyAsync(Type structureType, IEnumerable<object> items)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(items, "items").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertManyAsync(structureType, items);
            }
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
        {
            Ensure.That(json, "json").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertManyJson<T>(json);
            }
        }

        public virtual async Task InsertManyJsonAsync<T>(IEnumerable<string> json) where T : class
        {
            Ensure.That(json, "json").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertManyJsonAsync<T>(json);
            }
        }

        public virtual void InsertManyJson(Type structureType, IEnumerable<string> json)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(json, "json").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertManyJson(structureType, json);
            }
        }

        public virtual async Task InsertManyJsonAsync(Type structureType, IEnumerable<string> json)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(json, "json").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.InsertManyJsonAsync(structureType, json);
            }
        }

        public virtual void Update<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Update(item);
            }
        }

        public virtual async Task UpdateAsync<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.UpdateAsync(item);
            }
        }

        public virtual void Update(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Update(structureType, item);
            }
        }

        public virtual async Task UpdateAsync(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.UpdateAsync(structureType, item);
            }
        }

        public virtual void Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Ensure.That(id, "id").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Update(id, modifier, proceed);
            }
        }

        public virtual async Task UpdateAsync<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Ensure.That(id, "id").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.UpdateAsync(id, modifier, proceed);
            }
        }

        public virtual void UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.UpdateMany(predicate, modifier);
            }
        }

        public virtual async Task UpdateManyAsync<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.UpdateManyAsync(predicate, modifier);
            }
        }

        public virtual void Clear<T>() where T : class
        {
            using (var session = Db.BeginSession())
            {
                session.Clear<T>();
            }
        }

        public virtual async Task ClearAsync<T>() where T : class
        {
            using (var session = Db.BeginSession())
            {
                await session.ClearAsync<T>();
            }
        }

        public virtual void Clear(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Clear(structureType);
            }
        }

        public virtual async Task ClearAsync(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.ClearAsync(structureType);
            }
        }

        public virtual void DeleteAllExceptIds<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteAllExceptIds<T>(ids);
            }
        }

        public virtual async Task DeleteAllExceptIdsAsync<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                await session.DeleteAllExceptIdsAsync<T>(ids);
            }
        }

        public virtual void DeleteAllExceptIds(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteAllExceptIds(structureType, ids);
            }
        }

        public virtual async Task DeleteAllExceptIdsAsync(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                await session.DeleteAllExceptIdsAsync(structureType, ids);
            }
        }

        public virtual void DeleteById<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.DeleteById<T>(id);
            }
        }

        public virtual async Task DeleteByIdAsync<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.DeleteByIdAsync<T>(id);
            }
        }

        public virtual void DeleteById(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.DeleteById(structureType, id);
            }
        }

        public virtual async Task DeleteByIdAsync(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.DeleteByIdAsync(structureType, id);
            }
        }

        public virtual void DeleteByIds<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteByIds<T>(ids);
            }
        }

        public virtual async Task DeleteByIdsAsync<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                await session.DeleteByIdsAsync<T>(ids);
            }
        }

        public virtual void DeleteByIds(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteByIds(structureType, ids);
            }
        }

        public virtual async Task DeleteByIdsAsync(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                await session.DeleteByIdsAsync(structureType, ids);
            }
        }

        public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.DeleteByQuery(predicate);
            }
        }

        public virtual async Task DeleteByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                await session.DeleteByQueryAsync(predicate);
            }
        }
    }
}