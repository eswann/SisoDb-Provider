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
        protected readonly SessionEvents InternalEvents;
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;
        protected readonly ISqlStatements SqlStatements;

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
        public CacheConsumeModes CacheConsumeMode { get; protected set; }

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
            QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();
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
            return Try(() => new CheckoutByIdHelper(Db, DbClient).CheckoutById<T>(id));
        }

        public virtual async Task<T> CheckoutByIdAsync<T>(object id) where T : class
        {
            return await TryAsync(async () => await new CheckoutByIdHelper(Db, DbClient).CheckoutByIdAsync<T>(id));
        }

        public virtual IEnumerable<TId> GetIds<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() => new GetIdsHelper(Db, DbClient, QueryGenerator).GetIds<T, TId>(predicate));
        }

        public virtual async Task<IEnumerable<TId>> GetIdsAsync<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async () => await new GetIdsHelper(Db, DbClient, QueryGenerator).GetIdsAsync<T, TId>(predicate));
        }

        public virtual IEnumerable<object> GetIds<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() => new GetIdsHelper(Db, DbClient, QueryGenerator).GetIds<T, object>(predicate));
        }

        public virtual async Task<IEnumerable<object>> GetIdsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async () => await new GetIdsHelper(Db, DbClient, QueryGenerator).GetIdsAsync<T, object>(predicate));
        }

        public virtual T GetByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() => new GetByQueryHelper(Db, DbClient, QueryGenerator).GetByQueryAs(typeof(T), predicate));
        }

        public virtual async Task<T> GetByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async () => await new GetByQueryHelper(Db, DbClient, QueryGenerator).GetByQueryAsAsync(typeof(T), predicate));
        }

        public virtual T GetById<T>(object id) where T : class
        {
            return Try(() => new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAs<T>(typeof(T), id));
        }

        public virtual async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            return await TryAsync(async () => await new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsAsync<T>(typeof(T), id));
        }

        public virtual object GetById(Type structureType, object id)
        {
            return Try(() => new GetByIdHelper(Db, DbClient, QueryGenerator).GetById(structureType, id));
        }

        public virtual async Task<object> GetByIdAsync(Type structureType, object id)
        {
            return await TryAsync(async () => await new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsync(structureType, id));
        }

       
        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return Try(() => new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAs<TOut>(typeof(TContract), id));
        }

        public virtual async Task<TOut> GetByIdAsAsync<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return await TryAsync(async () => await new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsAsync<TOut>(typeof(TContract), id));
        }

        public TOut GetByIdAs<TOut>(Type structureType, object id) where TOut : class
        {
            return Try(() => new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAs<TOut>(structureType, id));
        }

        public async Task<TOut> GetByIdAsAsync<TOut>(Type structureType, object id) where TOut : class
        {
            return await TryAsync(async () => await new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsAsync<TOut>(structureType, id));
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Try(() => new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAs<T>(typeof(T), ids));
        }

        public virtual async Task<IEnumerable<T>> GetByIdsAsync<T>(params object[] ids) where T : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsAsync<T>(typeof(T), ids));
        }

        public virtual IEnumerable<object> GetByIds(Type structureType, params object[] ids)
        {
            return Try(() => new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIds(structureType, ids));
        }

        public virtual async Task<IEnumerable<object>> GetByIdsAsync(Type structureType, params object[] ids)
        {
            return await TryAsync(async () => await new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsync(structureType, ids));
        }



        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return Try(() => new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAs<TOut>(typeof(TContract), ids));
        }

        public virtual async Task<IEnumerable<TOut>> GetByIdsAsAsync<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsAsync<TOut>(typeof(TContract), ids));
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TOut>(Type structureType, params object[] ids)
            where TOut : class
        {
            return Try(() => new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAs<TOut>(structureType, ids));
        }

        public virtual async Task<IEnumerable<TOut>> GetByIdsAsAsync<TOut>(Type structureType, params object[] ids)
            where TOut : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsAsync<TOut>(structureType, ids));
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
        {
            return Try(() => new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsJson(typeof(T), id));
        }

        public virtual async Task<string> GetByIdAsJsonAsync<T>(object id) where T : class
        {
            return await TryAsync(async () => await new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsJsonAsync(typeof(T), id));
        }

        public virtual string GetByIdAsJson(Type structureType, object id)
        {
            return Try(() => new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsJson(structureType, id));
        }

        public virtual async Task<string> GetByIdAsJsonAsync(Type structureType, object id)
        {
            return await TryAsync(async () => await new GetByIdHelper(Db, DbClient, QueryGenerator).GetByIdAsJsonAsync(structureType, id));
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Try(() => new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsJson(typeof(T), ids));
        }

        public virtual async Task<IEnumerable<string>> GetByIdsAsJsonAsync<T>(params object[] ids) where T : class
        {
            return await TryAsync(async () => await new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsJsonAsync(typeof(T), ids));
        }

        public virtual IEnumerable<string> GetByIdsAsJson(Type structureType, params object[] ids)
        {
            return Try(() => new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsJson(structureType, ids));
        }

        public virtual async Task<IEnumerable<string>> GetByIdsAsJsonAsync(Type structureType, params object[] ids)
        {
            return await TryAsync(async () => await new GetByIdsHelper(Db, DbClient, QueryGenerator).GetByIdsAsJsonAsync(structureType, ids));
        }

        public virtual ISession Insert<T>(T item) where T : class
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Insert(typeof(T), item));

            return this;
        }

        public virtual async Task<ISession> InsertAsync<T>(T item) where T : class
        {
            await TryAsync(async() => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertAsync(typeof(T), item));

            return this;
        }

        public virtual ISession Insert(Type structureType, object item)
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Insert(structureType, item));

            return this;
        }

        public virtual async Task<ISession> InsertAsync(Type structureType, object item)
        {
            await TryAsync(async () => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertAsync(structureType, item));

            return this;
        }

    
        public virtual ISession InsertAs<T>(object item) where T : class
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertAs(typeof(T), item));

            return this;
        }

        public virtual async Task<ISession> InsertAsAsync<T>(object item) where T : class
        {
            await TryAsync(async () => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertAsAsync(typeof(T), item));

            return this;
        }

        public virtual ISession InsertAs(Type structureType, object item)
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertAs(structureType, item));

            return this;
        }

        public virtual async Task<ISession> InsertAsAsync(Type structureType, object item)
        {
            await TryAsync(async () => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertAsAsync(structureType, item));

            return this;
        }

        public virtual string InsertJson<T>(string json) where T : class
        {
            return Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertJson(typeof(T), json));
        }

        public virtual async Task<string> InsertJsonAsync<T>(string json) where T : class
        {
            return await TryAsync(async () => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertJsonAsync(typeof(T), json));
        }

        public virtual string InsertJson(Type structureType, string json)
        {
            return Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertJson(structureType, json));
        }

        public virtual async Task<string> InsertJsonAsync(Type structureType, string json)
        {
            return await TryAsync(async () => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertJsonAsync(structureType, json));
        }

        public virtual ISession InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertMany(typeof(T), items));
            return this;
        }

        public virtual async Task<ISession> InsertManyAsync<T>(IEnumerable<T> items) where T : class
        {
            await TryAsync(async() => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertManyAsync(typeof(T), items));
            return this;
        }

        public virtual ISession InsertMany(Type structureType, IEnumerable<object> items)
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertMany(structureType, items));
            return this;
        }

        public virtual async Task<ISession> InsertManyAsync(Type structureType, IEnumerable<object> items)
        {
            await TryAsync(async() => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertManyAsync(structureType, items));
            return this;
        }
        

        public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertManyJson(typeof(T), json));
        }

        public virtual async Task InsertManyJsonAsync<T>(IEnumerable<string> json) where T : class
        {
            await TryAsync(async () => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertManyJsonAsync(typeof(T), json));
        }

        public virtual void InsertManyJson(Type structureType, IEnumerable<string> json)
        {
            Try(() => new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertManyJson(structureType, json));
        }

        public virtual async Task InsertManyJsonAsync(Type structureType, IEnumerable<string> json)
        {
            await TryAsync(async() => await new InsertHelper(Db, DbClient, QueryGenerator, this, InternalEvents).InsertManyJsonAsync(structureType, json));
        }


        public virtual ISession Update<T>(T item) where T : class
        {
            Try(() => new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Update(typeof(T), item));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync<T>(T item) where T : class
        {
            await TryAsync(async () => await new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).UpdateAsync(typeof(T), item));

            return this;
        }

        public virtual ISession Update(Type structureType, object item)
        {
            Try(() => new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Update(structureType, item));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync(Type structureType, object item)
        {
            await TryAsync(async () => await new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).UpdateAsync(structureType, item));

            return this;
        }

        
        public virtual ISession Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Try(() => new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Update<T, T>(id, modifier, proceed));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            await TryAsync(async () => await new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).UpdateAsync<T, T>(id, modifier, proceed));

            return this;
        }

        public virtual ISession Update<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            Try(() => new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Update<TContract, TImpl>(id, modifier, proceed));

            return this;
        }

        public virtual async Task<ISession> UpdateAsync<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            await TryAsync(async () => await new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).UpdateAsync<TContract, TImpl>(id, modifier, proceed));

            return this;
        }

        

        public virtual ISession UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            Try(() => new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).UpdateMany(predicate, modifier));

            return this;
        }

        public virtual async Task<ISession> UpdateManyAsync<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            await TryAsync(async () => await new UpdateHelper(Db, DbClient, QueryGenerator, this, InternalEvents).UpdateManyAsync(predicate, modifier));

            return this;
        }

        public virtual ISession Clear<T>() where T : class
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Clear(typeof(T)));

            return this;
        }

        public virtual async Task<ISession> ClearAsync<T>() where T : class
        {
            await TryAsync(async() => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).ClearAsync(typeof(T)));

            return this;
        }

        public virtual ISession Clear(Type structureType)
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).Clear(structureType));

            return this;
        }

        public virtual async Task<ISession> ClearAsync(Type structureType)
        {
            await TryAsync(async() => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).ClearAsync(structureType));

            return this;
        }

        public virtual ISession DeleteAllExceptIds<T>(params object[] ids) where T : class
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteAllExceptIds(typeof(T), ids));

            return this;
        }

        public virtual async Task<ISession> DeleteAllExceptIdsAsync<T>(params object[] ids) where T : class
        {
            await TryAsync(async () => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteAllExceptIdsAsync(typeof(T), ids));

            return this;
        }

        public virtual ISession DeleteAllExceptIds(Type structureType, params object[] ids)
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteAllExceptIds(structureType, ids));

            return this;
        }

        public virtual async Task<ISession> DeleteAllExceptIdsAsync(Type structureType, params object[] ids)
        {
            await TryAsync(async () => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteAllExceptIdsAsync(structureType, ids));

            return this;
        }

        public virtual ISession DeleteById<T>(object id) where T : class
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteById(typeof(T), id));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdAsync<T>(object id) where T : class
        {
            await TryAsync(async () => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByIdAsync(typeof(T), id));

            return this;
        }

        public virtual ISession DeleteById(Type structureType, object id)
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteById(structureType, id));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdAsync(Type structureType, object id)
        {
            await TryAsync(async () => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByIdAsync(structureType, id));

            return this;
        }

        public virtual ISession DeleteByIds<T>(params object[] ids) where T : class
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByIds(typeof(T), ids));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdsAsync<T>(params object[] ids) where T : class
        {
            await TryAsync(async() => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByIdsAsync(typeof(T), ids));

            return this;
        }

        public virtual ISession DeleteByIds(Type structureType, params object[] ids)
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByIds(structureType, ids));

            return this;
        }

        public virtual async Task<ISession> DeleteByIdsAsync(Type structureType, params object[] ids)
        {
            await TryAsync(async() => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByIdsAsync(structureType, ids));

            return this;
        }


        public virtual ISession DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Try(() => new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByQuery(predicate));

            return this;
        }

        public virtual async Task<ISession> DeleteByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            await TryAsync(async() => await new DeleteHelper(Db, DbClient, QueryGenerator, this, InternalEvents).DeleteByQueryAsync(predicate));

            return this;
        }

    }
}