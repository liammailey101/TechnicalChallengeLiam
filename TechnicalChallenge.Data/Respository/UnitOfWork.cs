namespace TechnicalChallenge.Data.Respository
{
    /// <summary>
    /// Implements the unit of work pattern, providing a way to group multiple operations into a single transaction.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TechnicalChallengeDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UnitOfWork(TechnicalChallengeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        /// <example>
        /// var customerRepository = unitOfWork.GetRepository<Customer>();
        /// </example>
        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.TryGetValue(typeof(T), out var repository))
            {
                return (IRepository<T>)repository;
            }

            var newRepository = new GenericRepository<T>(_context);
            _repositories[typeof(T)] = newRepository;
            return newRepository;
        }

        /// <summary>
        /// Saves all changes made in the unit of work to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        /// <example>
        /// var changes = await unitOfWork.SaveChangesAsync();
        /// </example>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the unit of work and releases any resources it holds.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}