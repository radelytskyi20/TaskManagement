namespace TaskManagement.Persistence.QueryObjects.Common
{
    public static class GenericPaging
    {
        /// <summary>
        /// Paginates the queryable collection based on the specified page number and page size.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the queryable collection.</typeparam>
        /// <param name="query">The queryable collection to be paginated.</param>
        /// <param name="pageNumZeroStart">The page number (zero-based) to start the pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>An <see cref="IQueryable{T}"/> representing the paginated collection.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the page size is zero.</exception>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumZeroStart, int pageSize)
        {
            if (pageSize == 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize cannot be zero.");

            if (pageNumZeroStart != 0)
                query = query.Skip((pageNumZeroStart - 1) * pageSize);
               
            return query.Take(pageSize);
        }
    }
}
