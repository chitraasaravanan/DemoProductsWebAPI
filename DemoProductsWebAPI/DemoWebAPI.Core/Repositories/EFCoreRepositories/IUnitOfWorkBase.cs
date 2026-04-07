namespace DemoWebAPI.Core.Repositories.EFCoreRepositories
{
    /// <summary>
    /// Generic Unit of Work pattern for managing transaction boundaries and repository coordination.
    /// Implementations should coordinate multiple repositories and manage SaveChanges/commit operations.
    /// </summary>
    public interface IUnitOfWorkBase : IAsyncDisposable
    {
        /// <summary>
        /// Saves all pending changes to the underlying data store.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of entities changed.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes an operation within a transaction context.
        /// If the data store is not relational, executes the operation without transaction.
        /// </summary>
        /// <param name="operation">The async operation to execute within a transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    }
}
