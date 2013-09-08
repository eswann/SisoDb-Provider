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
        Task TryAsync(Func<Task> action);

        T Try<T>(Func<T> function);
        Task<T> TryAsync<T>(Func<Task<T>> function);

    }
}