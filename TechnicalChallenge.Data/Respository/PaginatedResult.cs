using System.ComponentModel.DataAnnotations;

namespace TechnicalChallenge.Data.Respository
{
    /// <summary>
    /// Represents a paginated result set.
    /// </summary>
    /// <typeparam name="T">The type of the data in the result set.</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Gets or sets the data in the current page.
        /// </summary>
        /// <value>The data in the current page.</value>
        public required IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Gets or sets the total count of items in the result set.
        /// </summary>
        /// <value>The total count of items in the result set.</value>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the index of the current page.
        /// </summary>
        /// <value>The index of the current page.</value>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the size of the current page.
        /// </summary>
        /// <value>The size of the current page.</value>
        public int PageSize { get; set; }
    }
}