using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace TechnicalChallenge.Data.Respository
{
    /// <summary>
    /// A generic repository for performing CRUD operations on entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Gets all entities, optionally including related entities.
        /// </summary>
        /// <param name="includeProperties">The related entities to include.</param>
        /// <returns>A list of all entities.</returns>
        /// <example>
        /// var allEntities = await repository.GetAllAsync();
        /// </example>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>[]? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets an entity by its identifier, optionally including related entities.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="includeProperties">The related entities to include.</param>
        /// <returns>The entity with the specified identifier, or null if not found.</returns>
        /// <example>
        /// var entity = await repository.GetByIdAsync(1);
        /// </example>
        public async Task<T?> GetByIdAsync(int id, Expression<Func<T, object>>[]? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        /// <summary>
        /// Finds entities that match the specified predicate, optionally including related entities.
        /// </summary>
        /// <param name="predicate">The predicate to filter entities.</param>
        /// <param name="includeProperties">The related entities to include.</param>
        /// <returns>A list of entities that match the predicate.</returns>
        /// <example>
        /// var entities = await repository.FindAsync(e => e.Name == "John");
        /// </example>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Gets the first entity that matches the specified predicate, optionally including related entities.
        /// </summary>
        /// <param name="predicate">The predicate to filter entities.</param>
        /// <param name="includeProperties">The related entities to include.</param>
        /// <returns>The first entity that matches the predicate, or null if not found.</returns>
        /// <example>
        /// var entity = await repository.FirstAsync(e => e.Name == "John");
        /// </example>
        public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <example>
        /// await repository.AddAsync(newEntity);
        /// </example>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Adds a range of new entities to the database.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <example>
        /// await repository.AddRangeAsync(newEntities);
        /// </example>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <example>
        /// await repository.UpdateAsync(existingEntity);
        /// </example>
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <example>
        /// await repository.DeleteAsync(1);
        /// </example>
        public async Task DeleteAsync(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <example>
        /// await repository.DeleteAsync(existingEntity);
        /// </example>
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets the count of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter entities.</param>
        /// <returns>The count of entities that match the predicate.</returns>
        /// <example>
        /// var count = await repository.CountAsync(e => e.Name == "John");
        /// </example>
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate != null)
            {
                return await _dbSet.CountAsync(predicate);
            }
            return await _dbSet.CountAsync();
        }

        /// <summary>
        /// Gets a paginated list of entities, optionally including related entities.
        /// </summary>
        /// <param name="pageIndex">The index of the page to retrieve.</param>
        /// <param name="pageSize">The size of the page to retrieve.</param>
        /// <param name="filter">The predicate to filter entities.</param>
        /// <param name="orderBy">The expression to order entities.</param>
        /// <param name="isAscending">Indicates whether the order is ascending.</param>
        /// <param name="includeProperties">The related entities to include.</param>
        /// <returns>A paginated result containing the entities.</returns>
        /// <example>
        /// var paginatedResult = await repository.GetPaginatedListAsync(1, 10, e => e.Name == "John", e => e.Name, true);
        /// </example>
        public async Task<PaginatedResult<T>> GetPaginatedListAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            bool isAscending = true,
            Expression<Func<T, object>>[]? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = isAscending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            var totalCount = await query.CountAsync();
            var data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResult<T>
            {
                Data = data,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}