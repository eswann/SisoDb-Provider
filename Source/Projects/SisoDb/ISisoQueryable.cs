using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SisoDb
{
	public interface ISisoQueryable<T> where T : class
	{
	    ISisoQueryable<T> Cacheable(); 

	    bool Any();
        Task<bool> AnyAsync();
        bool Any(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        int Count();
	    Task<int> CountAsync(); 
        int Count(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> expression);
        bool Exists(object id);
        Task<bool> ExistsAsync(object id);
        
        Task<T> FirstAsync();
        TResult FirstAs<TResult>() where TResult : class;
        Task<TResult> FirstAsAsync<TResult>() where TResult : class;
        string FirstAsJson();
        Task<string> FirstAsJsonAsync();
        T FirstOrDefault();
        Task<T> FirstOrDefaultAsync();
        TResult FirstOrDefaultAs<TResult>() where TResult : class;
        Task<TResult> FirstOrDefaultAsAsync<TResult>() where TResult : class;
        string FirstOrDefaultAsJson();
        Task<string> FirstOrDefaultAsJsonAsync();

        T Single();
        Task<T> SingleAsync();
        TResult SingleAs<TResult>() where TResult : class;
        Task<TResult> SingleAsAsync<TResult>() where TResult : class;
        string SingleAsJson();
        Task<string> SingleAsJsonAsync();
        T SingleOrDefault();
        Task<T> SingleOrDefaultAsync();
        TResult SingleOrDefaultAs<TResult>() where TResult : class;
        Task<TResult> SingleOrDefaultAsAsync<TResult>() where TResult : class;
        string SingleOrDefaultAsJson();
        Task<string> SingleOrDefaultAsJsonAsync();

        T[] ToArray();
        Task<T[]> ToArrayAsync();
        TResult[] ToArrayOf<TResult>() where TResult : class;
        Task<TResult[]> ToArrayOfAsync<TResult>() where TResult : class;
        TResult[] ToArrayOf<TResult>(TResult template) where TResult : class;
        Task<TResult[]> ToArrayOfAsync<TResult>(TResult template) where TResult : class;
        TResult[] ToArrayOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        Task<TResult[]> ToArrayOfAsync<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        string[] ToArrayOfJson();
        Task<string[]> ToArrayOfJsonAsync();

        IEnumerable<T> ToEnumerable();
        Task<IEnumerable<T>> ToEnumerableAsync();
        IEnumerable<TResult> ToEnumerableOf<TResult>() where TResult : class;
        Task<IEnumerable<TResult>> ToEnumerableOfAsync<TResult>() where TResult : class;
        IEnumerable<TResult> ToEnumerableOf<TResult>(TResult template) where TResult : class;
        Task<IEnumerable<TResult>> ToEnumerableOfAsync<TResult>(TResult template) where TResult : class;
        IEnumerable<TResult> ToEnumerableOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        Task<IEnumerable<TResult>> ToEnumerableOfAsync<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        IEnumerable<string> ToEnumerableOfJson();
        Task<IEnumerable<string>> ToEnumerableOfJsonAsync();

        IList<T> ToList();
        Task<IList<T>> ToListAsync();
        IList<TResult> ToListOf<TResult>() where TResult : class;
        Task<IList<TResult>> ToListOfAsync<TResult>() where TResult : class;
        IList<TResult> ToListOf<TResult>(TResult template) where TResult : class;
        Task<IList<TResult>> ToListOfAsync<TResult>(TResult template) where TResult : class;
        IList<TResult> ToListOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        Task<IList<TResult>> ToListOfAsync<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        IList<string> ToListOfJson();
        Task<IList<string>> ToListOfJsonAsync();

        ISisoQueryable<T> Skip(int numOfStructures);
		ISisoQueryable<T> Take(int numOfStructures);
        ISisoQueryable<T> Page(int pageIndex, int pageSize);
        ISisoQueryable<T> Where(params Expression<Func<T, bool>>[] expression);
        ISisoQueryable<T> OrderBy(params Expression<Func<T, object>>[] expressions);
        ISisoQueryable<T> OrderByDescending(params Expression<Func<T, object>>[] expressions);
	}
}