using TaskManagement.Domain.Constants.Query;

namespace TaskManagement.Domain.Contracts.Task
{
    /// <summary>
    /// Specifies the criteria by which tasks can be filtered.
    /// </summary>
    public enum TaskFilterBy
    {
        NoFilter,
        Status,
        Priority,
        UserId
    }

    /// <summary>
    /// Specifies the criteria by which tasks can be ordered.
    /// </summary>
    public enum TaskOrderBy
    {
        SimpleOrder,
        DueDate,
        DueDateDesc,
        Priority,
        PriorityDesc
    }

    /// <summary>
    /// Specifies the criteria by which tasks can be range-filtered.
    /// </summary>
    public enum TaskRangeFilterBy
    {
        NoFilter,
        DueDate
    }

    /// <summary>
    /// Represents the options for sorting, filtering, and paginating tasks.
    /// </summary>
    public class SortFilterTaskOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortFilterTaskOptions"/> class.
        /// </summary>
        /// <param name="sort">The sorting option.</param>
        /// <param name="status">The status filter options.</param>
        /// <param name="priority">The priority filter options.</param>
        /// <param name="start">The start date for range filtering.</param>
        /// <param name="end">The end date for range filtering.</param>
        /// <param name="userId">The user ID for filtering.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        public SortFilterTaskOptions(string? sort,
            IEnumerable<int>? status,
            IEnumerable<int>? priority,
            DateTime? start,
            DateTime? end,
            Guid userId,
            int pageNumber, int pageSize)
        {
            SetOrderOptions(sort);
            SetFilterOptions(status, priority, userId.ToString());
            SetRangeFilterOptions(start, end);

            PageNum = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Gets the sorting option.
        /// </summary>
        public TaskOrderBy OrderOption { get; private set; }

        /// <summary>
        /// Gets the filter options. Ket is the filter criteria, value is the filter value.
        /// </summary>
        public Dictionary<TaskFilterBy, ICollection<string>> FilterOptions { get; private set; } = new();

        /// <summary>
        /// Gets the range filter options. Key is the range filter criteria, value is the range filter values 
        /// where (string, string) => start, end.
        /// </summary>
        public Dictionary<TaskRangeFilterBy, ICollection<(string, string)>> FilterRangeOptions { get; private set; } = new();

        private int _pageNum = 1;
        /// <summary>
        /// Gets the page number for pagination.
        /// </summary>
        public int PageNum
        {
            get => _pageNum;
            private set
            {
                if (value <= 0) _pageNum = 1;
                _pageNum = value;
            }
        }

        private int _pageSize = 10;
        /// <summary>
        /// Gets the page size for pagination.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            private set
            {
                if (value <= 0) _pageSize = 10;
                _pageSize = value;
            }
        }

        /// <summary>
        /// Sets the sorting option based on the provided sort parameter.
        /// </summary>
        /// <param name="sort">The sorting parameter.</param>
        private void SetOrderOptions(string? sort)
        {
            OrderOption = sort switch
            {
                SortTaskParams.DueDate => TaskOrderBy.DueDate,
                SortTaskParams.DueDateDesc => TaskOrderBy.DueDateDesc,
                SortTaskParams.Priority => TaskOrderBy.Priority,
                SortTaskParams.PriorityDesc => TaskOrderBy.PriorityDesc,
                _ => TaskOrderBy.SimpleOrder,
            };
        }

        /// <summary>
        /// Sets the filter options based on the provided status, priority, and user ID.
        /// </summary>
        /// <param name="status">The status filter options.</param>
        /// <param name="priority">The priority filter options.</param>
        /// <param name="userId">The user ID for filtering.</param>
        private void SetFilterOptions(IEnumerable<int>? status, IEnumerable<int>? priority, string userId)
        {
            FilterOptions.Add(TaskFilterBy.UserId, new List<string> { userId });
            if (status is not null)
            {
                FilterOptions.Add(TaskFilterBy.Status, status.Select(s => s.ToString()).ToList());
            }
            if (priority is not null)
            {
                FilterOptions.Add(TaskFilterBy.Priority, priority.Select(p => p.ToString()).ToList());
            }
        }

        /// <summary>
        /// Sets the range filter options based on the provided start and end dates.
        /// </summary>
        /// <param name="start">The start date for range filtering.</param>
        /// <param name="end">The end date for range filtering.</param>
        private void SetRangeFilterOptions(DateTime? start, DateTime? end)
        {
            if (start is not null && end is not null)
            {
                FilterRangeOptions.Add(TaskRangeFilterBy.DueDate, new List<(string, string)>
                {
                    (start.Value.ToString(), end.Value.ToString())
                });
            }
        }
    }
}
