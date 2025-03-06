namespace TechnicalChallenge.Data.Respository
{
    /// <summary>
    /// Interface for the unit of work pattern, which provides a way to group multiple operations into a single transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        /// <example>
        /// var customerRepository = unitOfWork.GetRepository<Customer>();
        /// </example>
        IRepository<T> GetRepository<T>() where T : class;

        /// <summary>
        /// Saves all changes made in the unit of work to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        /// <example>
        /// var changes = await unitOfWork.SaveChangesAsync();
        /// </example>
        Task<int> SaveChangesAsync();
    }
}