namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IProductCartRepository ProductCarts { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IRefreshTokenRepository RefreshTokens { get; }

        /// <summary>
        /// Executes the provided operation inside a database transaction. Commits on success and rolls back on exception.
        /// </summary>
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    }
}
