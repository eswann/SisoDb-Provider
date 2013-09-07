using System;
using System.Threading.Tasks;

namespace SisoDb
{
    /// <summary>
    /// All operations within <see cref="DbSession"/> needs to be
    /// wrapped in a try, so that exceptions can be catched and
    /// the session could be marked as failed, so that automatic
    /// commits or rollbacks can be performed.
    /// </summary>
    public interface ISessionExecutionContext
    {
        ISession Session { get; }
        void Try(Action action);
        T Try<T>(Func<T> action);

        Task TryAsync(Func<Task> action);
        Task<T> TryAsync<T>(Func<Task<T>> action);
    }
}