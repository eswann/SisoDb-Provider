using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.DbSessionHelpers;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public abstract class DbSession : ISession
    {
        private readonly Guid _id;
        private readonly ISisoDatabase _db;
        private readonly IQueryEngine _queryEngine;
        private readonly IAdvanced _advanced;
        private readonly IDbQueryGenerator _queryGenerator;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;
        protected readonly ISqlStatements SqlStatements;
        protected internal readonly SessionEvents InternalEvents;


        public Guid Id { get { return _id; } }
        public ISessionExecutionContext ExecutionContext { get; private set; }
        public ISisoDatabase Db { get { return _db; } }
        public IDbClient DbClient { get; private set; }
        public SessionStatus Status { get; private set; }
        public bool IsAborted { get { return Status.IsAborted(); } }
        public bool HasFailed { get { return Status.IsFailed(); } }
        public ISessionEvents Events { get { return InternalEvents; } }
        public IQueryEngine QueryEngine { get { return _queryEngine; } }
        public IAdvanced Advanced { get { return _advanced; } }
        public CacheConsumeModes CacheConsumeMode { get; protected internal set; }
        public IDbQueryGenerator QueryGenerator { get { return _queryGenerator; } }
        

        protected DbSession(ISisoDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            _id = Guid.NewGuid();
            _db = db;
            DbClient = Db.ProviderFactory.GetTransactionalDbClient(Db);
            ExecutionContext = new SessionExecutionContext(this);
            Status = SessionStatus.Active;
            InternalEvents = new SessionEvents();
            SqlStatements = Db.ProviderFactory.GetSqlStatements();
            _queryGenerator = Db.ProviderFactory.GetDbQueryGenerator();
            SqlExpressionBuilder = Db.ProviderFactory.GetSqlExpressionBuilder();
            _queryEngine = new DbQueryEngine(ExecutionContext, QueryGenerator);
            _advanced = new DbSessionAdvanced(ExecutionContext, QueryGenerator, SqlExpressionBuilder);
            CacheConsumeMode = CacheConsumeModes.UpdateCacheWithDbResult;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            if (Status.IsDisposed())
                throw new ObjectDisposedException(typeof(DbSession).Name, ExceptionMessages.Session_AllreadyDisposed.Inject(Id, Db.Name));

            if (DbClient.IsFailed || Status.IsFailed())
                Status = SessionStatus.DisposedWithFailure;
            else if (DbClient.IsAborted || Status.IsAborted())
                Status = SessionStatus.DisposedAfterAbort;
            else
                Status = SessionStatus.Disposed;

            if (DbClient != null)
            {
                DbClient.Dispose();
                DbClient = null;
            }

            if (Status.IsAborted() || Status.IsFailed())
                InternalEvents.NotifyRolledback(Db, Id);
            else
                InternalEvents.NotifyCommitted(Db, Id);
        }

        public virtual void Abort()
        {
            if (HasFailed) return;
            //This method is allowed to not be wrapped in Try, since try makes use of it.
            Status = SessionStatus.Aborted;
            DbClient.Abort();
        }

        public virtual void MarkAsFailed()
        {
            //This method is allowed to not be wrapped in Try, since try makes use of it.
            Status = SessionStatus.Failed;
            DbClient.MarkAsFailed();
        }

        protected virtual void Try(Action action)
        {
            ExecutionContext.Try(action);
        }

        protected virtual T Try<T>(Func<T> action)
        {
            return ExecutionContext.Try(action);
        }

        protected virtual async Task TryAsync(Func<Task> action)
        {
            await ExecutionContext.TryAsync(action);
        }

        protected virtual async Task<T> TryAsync<T>(Func<Task<T>> function)
        {
            return await ExecutionContext.TryAsync(function);
        }

       

        public virtual IStructureSchema GetStructureSchema<T>() where T : class
        {
            return Try(() => OnGetStructureSchema(typeof(T)));
        }

        public virtual IStructureSchema GetStructureSchema(Type structureType)
        {
            return Try(() => OnGetStructureSchema(structureType));
        }

        protected virtual IStructureSchema OnGetStructureSchema(Type structureType)
        {
            return Db.StructureSchemas.GetSchema(structureType);
        }

        public virtual ISisoQueryable<T> Query<T>() where T : class
        {
            return Try(() => new SisoQueryable<T>(Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas), QueryEngine));
        }

        public virtual bool Any<T>() where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Any<T>();
        }

        public virtual async Task<bool> AnyAsync<T>() where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return await QueryEngine.AnyAsync<T>();
        }

        public virtual bool Any(Type structureType)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Any(structureType);
        }

        public virtual async Task<bool> AnyAsync(Type structureType)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return await QueryEngine.AnyAsync(structureType);
        }

        public virtual int Count<T>() where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Count<T>();
        }

        public virtual async Task<int> CountAsync<T>() where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return await QueryEngine.CountAsync<T>();
        }

        public virtual int Count(Type structureType)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Count(structureType);
        }

        public virtual async Task<int> CountAsync(Type structureType)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return await QueryEngine.CountAsync(structureType);
        }

        public virtual bool Exists<T>(object id) where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Exists<T>(id);
        }

        public virtual async Task<bool> ExistsAsync<T>(object id) where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return await QueryEngine.ExistsAsync<T>(id);
        }

        public virtual bool Exists(Type structureType, object id)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Exists(structureType, id);
        }

        public virtual async Task<bool> ExistsAsync(Type structureType, object id)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return await QueryEngine.ExistsAsync(structureType, id);
        }

        public virtual T CheckoutById<T>(object id) where T : class
        {
            return Try(() => new CheckoutByIdHelper(this).CheckoutById<T>(id));
        }

        public virtual async Task<T> CheckoutByIdAsync<T>(object id) where T : class
        {
            return await TryAsync(async () => await new CheckoutByIdHelper(this).CheckoutByIdAsync<T>(id));
        }

        public virtual IEnumerable<TId> GetIds<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() => new GetIdsHelper(this).GetIds<T, TId>(predicate));
        }

        public virtual async Task<IEnumerable<TId>> GetIdsAsync<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async () => await new GetIdsHelper(this).GetIdsAsync<T, TId>(predicate));
        }

        public virtual IEnumerable<object> GetIds<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() => new GetIdsHelper(this).GetIds<T, object>(predicate));
        }

        public virtual async Task<IEnumerable<object>> GetIdsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async () => await new GetIdsHelper(this).GetIdsAsync<T, object>(predicate));
        }

        public virtual T GetByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() => new GetByQueryHelper(this).GetByQueryAs(typeof(T), predicate));
        }

        public virtual async Task<T> GetByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async () => await new GetByQueryHelper(this).GetByQueryAsAsync(typeof(T), predicate));
        }

        public virtual T GetById<T>(object id) where T : class
        {
            return Try(() => new GetByIdHelper(this).GetByIdAs<T>(typeof(T), id));
        }

        public virtual async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            return await TryAsync(async () => await new GetByIdHelper(this).GetByIdAsAsync<T>(typeof(T), id));
        }

        public virtual object GetById(Type structureType, object id)
        {
            return Try(() => new GetByIdHelper(this).GetById(structureType, id));
        }

        public virtual async Task<object> GetByIdAsync(Type structureType, object id)
        {
            return await TryAsync(async () => await new GetByIdHelper(this).GetByIdAsync(structureType, id));
        }

       
        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return Try(() => new GetByIdHelper(this).GetByIdAs<TOut>(typeof(TContract), id));
        }

        public virtual async Task<TOut> GetByIdAsAsync<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return await TryAsync(async () => await new GetByIdHelper(this).GetByIdAsAsync<TOut>(typeof(TContract), id));
        }

        public TOut GetByIdAs<TOut>(Type structureType, object id) where TOut : class
        {
            return Try(() => new GetByIdHelper(this).GetByIdAs<TOut>(structureType, id));
        }

        public async Task<TOut> GetByIdAsAsync<TOut>(Type structureType, object id) where TOut : class
        {
            return await TryAsync(async () => await new GetByIdHelper(this).GetByIdAsAsync<TOut>(structureType, id));
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Try(() => new GetByIdsHelper(this).GetByIdsAs<T>(typeof(T), ids));
        }

        public virtual async Task<IEnumerable<T>> GetByIdsAsync<T>(params object[] ids) where T : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(this).GetByIdsAsAsync<T>(typeof(T), ids));
        }

        public virtual IEnumerable<object> GetByIds(Type structureType, params object[] ids)
        {
            return Try(() => new GetByIdsHelper(this).GetByIds(structureType, ids));
        }

        public virtual async Task<IEnumerable<object>> GetByIdsAsync(Type structureType, params object[] ids)
        {
            return await TryAsync(async () => await new GetByIdsHelper(this).GetByIdsAsync(structureType, ids));
        }



        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return Try(() => new GetByIdsHelper(this).GetByIdsAs<TOut>(typeof(TContract), ids));
        }

        public virtual async Task<IEnumerable<TOut>> GetByIdsAsAsync<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(this).GetByIdsAsAsync<TOut>(typeof(TContract), ids));
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TOut>(Type structureType, params object[] ids)
            where TOut : class
        {
            return Try(() => new GetByIdsHelper(this).GetByIdsAs<TOut>(structureType, ids));
        }

        public virtual async Task<IEnumerable<TOut>> GetByIdsAsAsync<TOut>(Type structureType, params object[] ids)
            where TOut : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(this).GetByIdsAsAsync<TOut>(structureType, ids));
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
        {
            return Try(() => new GetByIdHelper(this).GetByIdAsJson(typeof(T), id));
        }

        public virtual async Task<string> GetByIdAsJsonAsync<T>(object id) where T : class
        {
            return await TryAsync(async () => await new GetByIdHelper(this).GetByIdAsJsonAsync(typeof(T), id));
        }

        public virtual string GetByIdAsJson(Type structureType, object id)
        {
            return Try(() => new GetByIdHelper(this).GetByIdAsJson(structureType, id));
        }

        public virtual async Task<string> GetByIdAsJsonAsync(Type structureType, object id)
        {
            return await TryAsync(async () => await new GetByIdHelper(this).GetByIdAsJsonAsync(structureType, id));
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Try(() => new GetByIdsHelper(this).GetByIdsAsJson(typeof(T), ids));
        }

        public virtual async Task<IEnumerable<string>> GetByIdsAsJsonAsync<T>(params object[] ids) where T : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(this).GetByIdsAsJsonAsync(typeof(T), ids));
        }

        public virtual IEnumerable<string> GetByIdsAsJson(Type structureType, params object[] ids)
        {
            return Try(() => new GetByIdsHelper(this).GetByIdsAsJson(structureType, ids));
        }

        public virtual async Task<IEnumerable<string>> GetByIdsAsJsonAsync(Type structureType, params object[] ids)
        {
            return await TryAsync(async () => await new GetByIdsHelper(this).GetByIdsAsJsonAsync(structureType, ids));
        }

        public virtual ISession Insert<T>(T item) where T : class
        {
            Try(() => new InsertHelper(this).Insert(typeof(T), item));

            return this;
        }

        public virtual async Task<ISession> InsertAsync<T>(T item) where T : class
        {
            await TryAsync(async() => await new InsertHelper(this).InsertAsync(typeof(T), item));

            return this;
        }

        public virtual ISession Insert(Type structureType, object item)
        {
            Try(() => new InsertHelper(this).Insert(structureType, item));

            return this;
        }

        public virtual async Task<ISession> InsertAsync(Type structureType, object item)
        {
            await TryAsync(async () => await new InsertHelper(this).InsertAsync(structureType, item));

            return this;
        }

    
        public virtual ISession InsertAs<T>(object item) where T : class
        {
            Try(() => new InsertHelper(this).InsertAs(typeof(T), item));

            return this;
        }

        public virtual async Task<ISession> InsertAsAsync<T>(object item) where T : class
        {
            await TryAsync(async () => await new InsertHelper(this).InsertAsAsync(typeof(T), item));

            return this;
        }

        public virtual ISession InsertAs(Type structureType, object item)
        {
            Try(() => new InsertHelper(this).InsertAs(structureType, item));

            return this;
        }

        public virtual async Task<ISession> InsertAsAsync(Type structureType, object item)
        {
            await TryAsync(async () => await new InsertHelper(this).InsertAsAsync(structureType, item));

            return this;
        }

        public virtual string InsertJson<T>(string json) where T : class
        {
            return Try(() => new InsertHelper(this).InsertJson(typeof(T), json));
        }

        public virtual async Task<string> InsertJsonAsync<T>(string json) where T : class
        {
            return await TryAsync(async () => await new InsertHelper(this).InsertJsonAsync(typeof(T), json));
        }

        public virtual string InsertJson(Type structureType, string json)
        {
            return Try(() => new InsertHelper(this).InsertJson(structureType, json));
        }

        public virtual async Task<string> InsertJsonAsync(Type structureType, string json)
        {
            return await TryAsync(async () => await new InsertHelper(this).InsertJsonAsync(structureType, json));
        }

        public virtual ISession InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Try(() => new InsertHelper(this).InsertMany(typeof(T), items));
            return this;
        }

        public virtual async Task<ISession> InsertManyAsync<T>(IEnumerable<T> items) where T : class
        {
            await TryAsync(async() => await new InsertHelper(this).InsertManyAsync(typeof(T), items));
            return this;
        }

        public virtual ISession InsertMany(Type structureType, IEnumerable<object> items)
        {
            Try(() => new InsertHelper(this).InsertMany(structureType, items));
            return this;
        }

        public virtual async Task<ISession> InsertManyAsync(Type structureType, IEnumerable<object> items)
        {
            await TryAsync(async() => await new InsertHelper(this).InsertManyAsync(structureType, items));
            return this;
        }
        

        public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
        {
            Try(() => new InsertHelper(this).InsertManyJson(typeof(T), json));
        }

        public virtual async Task InsertManyJsonAsync<T>(IEnumerable<string> json) where T : class
        {
            await TryAsync(async () => await new InsertHelper(this).InsertManyJsonAsync(typeof(T), json));
        }

        public virtual void InsertManyJson(Type structureType, IEnumerable<string> json)
        {
            Try(() => new InsertHelper(this).InsertManyJson(structureType, json));
        }

        public virtual async Task InsertManyJsonAsync(Type structureType, IEnumerable<string> json)
        {
            await TryAsync(async() => await new InsertHelper(this).InsertManyJsonAsync(structureType, json));
        }


        public virtual ISession Update<T>(T item) where T : class
        {
            Try(() => new UpdateHelper(this).Update(typeof(T), item));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync<T>(T item) where T : class
        {
            await TryAsync(async () => await new UpdateHelper(this).UpdateAsync(typeof(T), item));

            return this;
        }

        public virtual ISession Update(Type structureType, object item)
        {
            Try(() => new UpdateHelper(this).Update(structureType, item));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync(Type structureType, object item)
        {
            await TryAsync(async () => await new UpdateHelper(this).UpdateAsync(structureType, item));

            return this;
        }

        
        public virtual ISession Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Try(() => new UpdateHelper(this).Update<T, T>(id, modifier, proceed));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            await TryAsync(async () => await new UpdateHelper(this).UpdateAsync<T, T>(id, modifier, proceed));

            return this;
        }

        public virtual ISession Update<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            Try(() => new UpdateHelper(this).Update<TContract, TImpl>(id, modifier, proceed));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            await TryAsync(async () => await new UpdateHelper(this).UpdateAsync<TContract, TImpl>(id, modifier, proceed));

            return this;
        }

        

        public virtual ISession UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            Try(() => new UpdateHelper(this).UpdateMany(predicate, modifier));

            return this;
        }

        public virtual async Task<ISession> UpdateManyAsync<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            await TryAsync(async () => await new UpdateHelper(this).UpdateManyAsync(predicate, modifier));

            return this;
        }

        public virtual ISession Clear<T>() where T : class
        {
            Try(() => new DeleteHelper(this).Clear(typeof(T)));

            return this;
        }

        public virtual async Task<ISession> ClearAsync<T>() where T : class
        {
            await TryAsync(async() => await new DeleteHelper(this).ClearAsync(typeof(T)));

            return this;
        }

        public virtual ISession Clear(Type structureType)
        {
            Try(() => new DeleteHelper(this).Clear(structureType));

            return this;
        }

        public virtual async Task<ISession> ClearAsync(Type structureType)
        {
            await TryAsync(async() => await new DeleteHelper(this).ClearAsync(structureType));

            return this;
        }

        public virtual ISession DeleteAllExceptIds<T>(params object[] ids) where T : class
        {
            Try(() => new DeleteHelper(this).DeleteAllExceptIds(typeof(T), ids));

            return this;
        }

        public virtual async Task<ISession> DeleteAllExceptIdsAsync<T>(params object[] ids) where T : class
        {
            await TryAsync(async () => await new DeleteHelper(this).DeleteAllExceptIdsAsync(typeof(T), ids));

            return this;
        }

        public virtual ISession DeleteAllExceptIds(Type structureType, params object[] ids)
        {
            Try(() => new DeleteHelper(this).DeleteAllExceptIds(structureType, ids));

            return this;
        }

        public virtual async Task<ISession> DeleteAllExceptIdsAsync(Type structureType, params object[] ids)
        {
            await TryAsync(async () => await new DeleteHelper(this).DeleteAllExceptIdsAsync(structureType, ids));

            return this;
        }

        public virtual ISession DeleteById<T>(object id) where T : class
        {
            Try(() => new DeleteHelper(this).DeleteById(typeof(T), id));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdAsync<T>(object id) where T : class
        {
            await TryAsync(async () => await new DeleteHelper(this).DeleteByIdAsync(typeof(T), id));

            return this;
        }

        public virtual ISession DeleteById(Type structureType, object id)
        {
            Try(() => new DeleteHelper(this).DeleteById(structureType, id));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdAsync(Type structureType, object id)
        {
            await TryAsync(async () => await new DeleteHelper(this).DeleteByIdAsync(structureType, id));

            return this;
        }

        public virtual ISession DeleteByIds<T>(params object[] ids) where T : class
        {
            Try(() => new DeleteHelper(this).DeleteByIds(typeof(T), ids));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdsAsync<T>(params object[] ids) where T : class
        {
            await TryAsync(async() => await new DeleteHelper(this).DeleteByIdsAsync(typeof(T), ids));

            return this;
        }

        public virtual ISession DeleteByIds(Type structureType, params object[] ids)
        {
            Try(() => new DeleteHelper(this).DeleteByIds(structureType, ids));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdsAsync(Type structureType, params object[] ids)
        {
            await TryAsync(async() => await new DeleteHelper(this).DeleteByIdsAsync(structureType, ids));

            return this;
        }


        public virtual ISession DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Try(() => new DeleteHelper(this).DeleteByQuery(predicate));

            return this;
        }

        public virtual async Task<ISession> DeleteByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            await TryAsync(async() => await new DeleteHelper(this).DeleteByQueryAsync(predicate));

            return this;
        }

    }
}