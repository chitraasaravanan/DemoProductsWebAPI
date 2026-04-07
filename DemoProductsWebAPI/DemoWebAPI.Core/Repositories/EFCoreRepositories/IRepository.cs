using System.Linq.Expressions;

namespace DemoWebAPI.Core.Repositories.EFCoreRepositories
{
    /// <summary>
    /// Generic repository interface for common CRUD operations.
    /// This can be implemented by specific repository types.
    /// </summary>
    /// <typeparam name="TEntity">The entity type managed by this repository.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The entity if found; otherwise null.</returns>
        Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of all entities.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of matching entities.</returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if any entity matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if any entity matches; otherwise false.</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The count of matching entities.</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
