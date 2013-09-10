using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.EnsureThat;

namespace SisoDb.Querying
{
	public class SisoQueryable<T> : ISisoQueryable<T> where T : class
	{
	    protected readonly IQueryEngine QueryEngine;
		protected readonly IQueryBuilder<T> QueryBuilder;
	    
	    public SisoQueryable(IQueryBuilder<T> queryBuilder, IQueryEngine queryEngine)
		{
            Ensure.That(queryBuilder, "queryBuilder").IsNotNull();
            Ensure.That(queryEngine, "queryEngine").IsNotNull();

            QueryBuilder = queryBuilder;
            QueryEngine = queryEngine;
		}

	    public virtual ISisoQueryable<T> Cacheable()
	    {
	        QueryBuilder.MakeCacheable();
	        
            return this;
	    }

	    public virtual bool Any()
        {
            return QueryBuilder.IsEmpty 
                ? QueryEngine.Any<T>()
                : QueryEngine.Any<T>(QueryBuilder.Build());
        }

        public virtual async Task<bool> AnyAsync()
        {
            return QueryBuilder.IsEmpty
                ? await QueryEngine.AnyAsync<T>()
                : await QueryEngine.AnyAsync<T>(QueryBuilder.Build());
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            PrepareAnyQueryBuilder(expression);

            return QueryEngine.Any<T>(QueryBuilder.Build());
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            PrepareAnyQueryBuilder(expression);

            return await QueryEngine.AnyAsync<T>(QueryBuilder.Build());
        }

	    private void PrepareAnyQueryBuilder(Expression<Func<T, bool>> expression)
	    {
	        Ensure.That(expression, "expression").IsNotNull();

	        QueryBuilder.Clear();
	        QueryBuilder.Where(expression);
	    }

	    public virtual int Count()
        {
            return QueryBuilder.IsEmpty
                ? QueryEngine.Count<T>()
                : QueryEngine.Count<T>(QueryBuilder.Build());
        }

        public virtual async Task<int> CountAsync()
        {
            return QueryBuilder.IsEmpty
                ? await QueryEngine.CountAsync<T>()
                : await QueryEngine.CountAsync<T>(QueryBuilder.Build());
        }

        public virtual int Count(Expression<Func<T, bool>> expression)
        {
            PrepareCountQueryBuilder(expression);

            return QueryEngine.Count<T>(QueryBuilder.Build());
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            PrepareCountQueryBuilder(expression);

            return await QueryEngine.CountAsync<T>(QueryBuilder.Build());
        }

	    private void PrepareCountQueryBuilder(Expression<Func<T, bool>> expression)
	    {
	        Ensure.That(expression, "expression").IsNotNull();

	        QueryBuilder.Clear();
	        QueryBuilder.Where(expression);
	    }

	    public virtual bool Exists(object id)
	    {
	        Ensure.That(id, "id").IsNotNull();

	        return QueryEngine.Exists<T>(id);
	    }

        public virtual async Task<bool> ExistsAsync(object id)
        {
            Ensure.That(id, "id").IsNotNull();

            return await QueryEngine.ExistsAsync<T>(id);
        }

	    public virtual T First()
	    {
            OnBeforeFirst();
			return ToEnumerable().First();
		}

        public virtual async Task<T> FirstAsync()
        {
            OnBeforeFirst();
            return (await ToEnumerableAsync()).First();
        }

		public virtual TResult FirstAs<TResult>() where TResult : class
		{
            OnBeforeFirst();
			return ToEnumerableOf<TResult>().First();
		}

        public virtual async Task<TResult> FirstAsAsync<TResult>() where TResult : class
        {
            OnBeforeFirst();
            return (await ToEnumerableOfAsync<TResult>()).First();
        }

		public virtual string FirstAsJson()
		{
            OnBeforeFirst();
			return ToEnumerableOfJson().First();
		}

        public virtual async Task<string> FirstAsJsonAsync()
        {
            OnBeforeFirst();
            return (await ToEnumerableOfJsonAsync()).First();
        }

		public virtual T FirstOrDefault()
		{
            OnBeforeFirst();
			return ToEnumerable().FirstOrDefault();
		}

        public virtual async Task<T> FirstOrDefaultAsync()
        {
            OnBeforeFirst();
            return (await ToEnumerableAsync()).FirstOrDefault();
        }

		public virtual TResult FirstOrDefaultAs<TResult>() where TResult : class
		{
            OnBeforeFirst();
			return ToEnumerableOf<TResult>().FirstOrDefault();
		}

        public virtual async Task<TResult> FirstOrDefaultAsAsync<TResult>() where TResult : class
        {
            OnBeforeFirst();
            return (await ToEnumerableOfAsync<TResult>()).FirstOrDefault();
        }

		public virtual string FirstOrDefaultAsJson()
		{
            OnBeforeFirst();
			return ToEnumerableOfJson().FirstOrDefault();
		}

        public virtual async Task<string> FirstOrDefaultAsJsonAsync()
        {
            OnBeforeFirst();
            return (await ToEnumerableOfJsonAsync()).FirstOrDefault();
        }

        protected virtual void OnBeforeFirst()
        {
            QueryBuilder.First();
        }

		public virtual T Single()
		{
            OnBeforeSingle();
			return ToEnumerable().Single();
		}

        public virtual async Task<T> SingleAsync()
        {
            OnBeforeSingle();
            return (await ToEnumerableAsync()).Single();
        }

		public virtual TResult SingleAs<TResult>() where TResult : class
		{
            OnBeforeSingle();
			return ToEnumerableOf<TResult>().Single();
		}

        public virtual async Task<TResult> SingleAsAsync<TResult>() where TResult : class
        {
            OnBeforeSingle();
            return (await ToEnumerableOfAsync<TResult>()).Single();
        }

		public virtual string SingleAsJson()
		{
            OnBeforeSingle();
			return ToEnumerableOfJson().Single();
		}

        public virtual async Task<string> SingleAsJsonAsync()
        {
            OnBeforeSingle();
            return (await ToEnumerableOfJsonAsync()).Single();
        }

		public virtual T SingleOrDefault()
		{
            OnBeforeSingle();
			return ToEnumerable().SingleOrDefault();
		}

        public virtual async Task<T> SingleOrDefaultAsync()
        {
            OnBeforeSingle();
            return (await ToEnumerableAsync()).SingleOrDefault();
        }

		public virtual TResult SingleOrDefaultAs<TResult>() where TResult : class 
		{
            OnBeforeSingle();
			return ToEnumerableOf<TResult>().SingleOrDefault();
		}

        public virtual async Task<TResult> SingleOrDefaultAsAsync<TResult>() where TResult : class
        {
            OnBeforeSingle();
            return (await ToEnumerableOfAsync<TResult>()).SingleOrDefault();
        }

		public virtual string SingleOrDefaultAsJson()
		{
            OnBeforeSingle();
			return ToEnumerableOfJson().SingleOrDefault();
		}

        public virtual async Task<string> SingleOrDefaultAsJsonAsync()
        {
            OnBeforeSingle();
            return (await ToEnumerableOfJsonAsync()).SingleOrDefault();
        }

        protected virtual void OnBeforeSingle()
        {
            QueryBuilder.Single();
        }

        public virtual T[] ToArray()
        {
            return ToEnumerable().ToArray();
        }

        public virtual async Task<T[]> ToArrayAsync()
        {
            return (await ToEnumerableAsync()).ToArray();
        }

        public virtual TResult[] ToArrayOf<TResult>() where TResult : class
        {
            return ToEnumerableOf<TResult>().ToArray();
        }

        public virtual async Task<TResult[]> ToArrayOfAsync<TResult>() where TResult : class
        {
            return (await ToEnumerableOfAsync<TResult>()).ToArray();
        }

        public virtual TResult[] ToArrayOf<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return ToEnumerableOf(template).ToArray();
        }

        public virtual async Task<TResult[]> ToArrayOfAsync<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return (await ToEnumerableOfAsync(template)).ToArray();
        }

        public virtual TResult[] ToArrayOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
	    {
	        return ToEnumerableOf(projection).ToArray();
	    }

        public virtual async Task<TResult[]> ToArrayOfAsync<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            return (await ToEnumerableOfAsync(projection)).ToArray();
        }

	    public virtual string[] ToArrayOfJson()
        {
            return ToEnumerableOfJson().ToArray();
        }

        public virtual async Task<string[]> ToArrayOfJsonAsync()
        {
            return (await ToEnumerableOfJsonAsync()).ToArray();
        }

        public virtual IEnumerable<T> ToEnumerable()
        {
            return QueryEngine.Query<T>(QueryBuilder.Build());
        }

        public virtual async Task<IEnumerable<T>> ToEnumerableAsync()
        {
            return await QueryEngine.QueryAsync<T>(QueryBuilder.Build());
        }

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>() where TResult : class
        {
            return QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build());
        }

        public virtual async Task<IEnumerable<TResult>> ToEnumerableOfAsync<TResult>() where TResult : class
        {
            return await QueryEngine.QueryAsAsync<T, TResult>(QueryBuilder.Build());
        }

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build());
        }

        public virtual async Task<IEnumerable<TResult>> ToEnumerableOfAsync<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return await QueryEngine.QueryAsAsync<T, TResult>(QueryBuilder.Build());
        }

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            Ensure.That(projection, "projection").IsNotNull();

            return QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build());
        }

        public virtual async Task<IEnumerable<TResult>> ToEnumerableOfAsync<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            Ensure.That(projection, "projection").IsNotNull();

            return await QueryEngine.QueryAsAsync<T, TResult>(QueryBuilder.Build());
        }

        public virtual IEnumerable<string> ToEnumerableOfJson()
        {
            return QueryEngine.QueryAsJson<T>(QueryBuilder.Build());
        }

        public virtual async Task<IEnumerable<string>> ToEnumerableOfJsonAsync()
        {
            return await QueryEngine.QueryAsJsonAsync<T>(QueryBuilder.Build());
        }

        public virtual IList<T> ToList()
        {
            return ToEnumerable().ToList();
        }

        public virtual async Task<IList<T>> ToListAsync()
        {
            return (await ToEnumerableAsync()).ToList();
        }

        public virtual IList<TResult> ToListOf<TResult>() where TResult : class
        {
            return ToEnumerableOf<TResult>().ToList();
        }

        public virtual async Task<IList<TResult>> ToListOfAsync<TResult>() where TResult : class
        {
            return (await ToEnumerableOfAsync<TResult>()).ToList();
        }
        
        public virtual IList<TResult> ToListOf<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return ToEnumerableOf(template).ToList();
        }

        public virtual async Task<IList<TResult>> ToListOfAsync<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return (await ToEnumerableOfAsync(template)).ToList();
        }

        public virtual IList<TResult> ToListOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            Ensure.That(projection, "projection").IsNotNull();

            return ToEnumerableOf(projection).ToList();
        }

        public virtual async Task<IList<TResult>> ToListOfAsync<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            Ensure.That(projection, "projection").IsNotNull();

            return (await ToEnumerableOfAsync(projection)).ToList();
        }

        public virtual IList<string> ToListOfJson()
        {
            return ToEnumerableOfJson().ToList();
        }

        public virtual async Task<IList<string>> ToListOfJsonAsync()
        {
            return (await ToEnumerableOfJsonAsync()).ToList();
        }

	    public virtual ISisoQueryable<T> Skip(int numOfStructures)
	    {
	        QueryBuilder.Skip(numOfStructures);

	        return this;
	    }

	    public virtual ISisoQueryable<T> Take(int numOfStructures)
		{
			QueryBuilder.Take(numOfStructures);
			
			return this;
		}

		public virtual ISisoQueryable<T> Page(int pageIndex, int pageSize)
		{
			QueryBuilder.Page(pageIndex, pageSize);

			return this;
		}

		public virtual ISisoQueryable<T> Where(params Expression<Func<T, bool>>[] expressions)
		{
		    Ensure.That(expressions, "expressions").HasItems();

			QueryBuilder.Where(expressions);
			
			return this;
		}

		public virtual ISisoQueryable<T> OrderBy(params Expression<Func<T, object>>[] expressions)
		{
            Ensure.That(expressions, "expressions").HasItems();

			QueryBuilder.OrderBy(expressions);

			return this;
		}

		public virtual ISisoQueryable<T> OrderByDescending(params Expression<Func<T, object>>[] expressions)
		{
            Ensure.That(expressions, "expressions").HasItems();

			QueryBuilder.OrderByDescending(expressions);

			return this;
		}
	}
}