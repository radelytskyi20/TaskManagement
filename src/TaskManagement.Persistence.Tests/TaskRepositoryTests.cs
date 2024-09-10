using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Domain.Constants.Query;
using TaskManagement.Domain.Contracts.Task;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Persistence.Repositories;
using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;
using TaskPriority = TaskManagement.Domain.Entities.TaskPriority;

namespace TaskManagement.Persistence.Tests
{
    [TestFixture]
    public class TaskRepositoryTests
    {
        private readonly Fixture _fixture = new();
        private IServiceProvider serviceProvider;

        public TaskRepositoryTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            services.AddTransient<ITaskRepository, TaskRepository>();
            serviceProvider = services.BuildServiceProvider();
        }

        [Test]
        [TestCase(10000, SortTaskParams.DueDate)] //amount, sortParameter
        [TestCase(10000, SortTaskParams.DueDateDesc)]
        [TestCase(10000, SortTaskParams.Priority)]
        [TestCase(10000, SortTaskParams.PriorityDesc)]
        [TestCase(10000, null)] //simple order => created at desc => for pagination
        public async Task GIVEN_Task_Repository_WHEN_I_sort_THEN_tasks_are_sorted_correctly(int amount, string? sortParameter)
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var tasks = GetTestData(amount);
            SeedData(dbContext, tasks);

            var sortFilterOptions = new SortFilterTaskOptions(
                sort: sortParameter,
                status: null,
                priority: null,
                start: null,
                end: null,
                tasks.First().UserId,
                pageNumber: 1,
                pageSize: amount
            );

            //Act
            var result = await taskRepository.GetAll(sortFilterOptions).ToListAsync();
            var sorted = sortParameter switch
            {
                SortTaskParams.DueDate => tasks.OrderBy(t => t.DueDate),
                SortTaskParams.DueDateDesc => tasks.OrderByDescending(t => t.DueDate),
                SortTaskParams.Priority => tasks.OrderBy(t => t.Priority),
                SortTaskParams.PriorityDesc => tasks.OrderByDescending(t => t.Priority),
                _ => tasks.OrderByDescending(t => t.CreatedAt)
            };
            
            //Assert
            CollectionAssert.AreEqual(sorted.Select(x => x.Id), result.Select(x => x.Id));
        }

        [Test]
        [TestCase(10000, TaskFilterBy.NoFilter, null)]
        [TestCase(10000, TaskFilterBy.Status, "1", "2")]
        [TestCase(10000, TaskFilterBy.Priority, "1", "2")]
        [TestCase(10000, TaskFilterBy.UserId, null)]
        public async Task GIVEN_Task_Repository_WHEN_I_filter_tasks_THEN_tasks_are_filtered_correctly(int amount, TaskFilterBy filterBy, params string[]? filters)
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var tasks = GetTestData(amount);
            var userId = tasks.First().UserId;
            SeedData(dbContext, tasks);

            SortFilterTaskOptions sortFilterOptions;
            IEnumerable<TaskEntity> filtered;
            
            switch (filterBy)
            {
                case TaskFilterBy.NoFilter:
                    sortFilterOptions = new SortFilterTaskOptions(
                        sort: null,
                        status: null,
                        priority: null,
                        start: null,
                        end: null,
                        userId,
                        pageNumber: 1,
                        pageSize: amount
                    );
                    filtered = tasks;
                    break;
                
                case TaskFilterBy.Status:
                    var statusFilters = filters?.Select(f => int.Parse(f)) ?? Enumerable.Empty<int>();
                    sortFilterOptions = new SortFilterTaskOptions(
                        sort: null,
                        status: statusFilters,
                        priority: null,
                        start: null,
                        end: null,
                        userId,
                        pageNumber: 1,
                        pageSize: amount
                    );
                    filtered = tasks.Where(t => statusFilters.Contains((int)t.Status));
                    break;
                
                case TaskFilterBy.Priority:
                    var priorityFilters = filters?.Select(f => int.Parse(f)) ?? Enumerable.Empty<int>();
                    sortFilterOptions = new SortFilterTaskOptions(
                        sort: null,
                        status: null,
                        priority: priorityFilters,
                        start: null,
                        end: null,
                        userId,
                        pageNumber: 1,
                        pageSize: amount
                    );
                    filtered = tasks.Where(t => priorityFilters.Contains((int)t.Priority));
                    break;
                
                case TaskFilterBy.UserId:
                    var userIdFilter = Guid.TryParse(filters?.FirstOrDefault(), out var parsedUserId) ?
                        parsedUserId : userId;
                    
                    sortFilterOptions = new SortFilterTaskOptions(
                        sort: null,
                        status: null,
                        priority: null,
                        start: null,
                        end: null,
                        userIdFilter,
                        pageNumber: 1,
                        pageSize: amount
                    );
                    filtered = tasks.Where(t => t.UserId == userIdFilter);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
            }

            //follow to auth rulse, only user can see his tasks so we always filter by userId
            //always order by created at desc for pagination
            filtered = filtered
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt);

            //Act
            var result = await taskRepository.GetAll(sortFilterOptions).ToListAsync();

            //Assert
            CollectionAssert.AreEqual(filtered.Select(x => x.Id), result.Select(x => x.Id));
        }

        [Test]
        [TestCase(10000)]
        [TestCase(100000)]
        public async Task GIVEN_Task_Repository_WHEN_I_apply_multiple_filters_THEN_tasks_are_filtered_correctly(int amount)
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var tasks = GetTestData(amount);
            var userId = tasks.First().UserId;
            SeedData(dbContext, tasks);

            var statusFilters = new[] { 1, 2 };
            var priorityFilters = new[] { 1, 2 };

            var sortFilterOptions = new SortFilterTaskOptions(
                sort: null,
                status: statusFilters,
                priority: priorityFilters,
                start: null,
                end: null,
                userId,
                pageNumber: 1,
                pageSize: amount
            );

            //t => (t.Status == 1 || t.Status == 2) && (t.Priority == 1 || t.Priority == 2) && t.UserId == userId
            var filtered = tasks
                .Where(t => statusFilters.Contains((int)t.Status))
                .Where(t => priorityFilters.Contains((int)t.Priority))
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt);

            //Act
            var result = await taskRepository.GetAll(sortFilterOptions).ToListAsync();

            //Assert
            CollectionAssert.AreEqual(filtered.Select(x => x.Id), result.Select(x => x.Id));
        }

        [Test]
        [TestCase(10000)]
        [TestCase(100000)]
        public async Task GIVEN_Task_Repository_WHEN_I_apply_range_filters_THEN_tasks_are_filtered_correctly(int amount)
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var tasks = GetTestData(amount);
            var userId = tasks.First().UserId;
            SeedData(dbContext, tasks);

            var start = _fixture.Create<DateTime>();
            var end = start.AddDays(_fixture.Create<int>());

            var sortFilterOptions = new SortFilterTaskOptions(
                sort: null,
                status: null,
                priority: null,
                start: start,
                end: end,
                userId,
                pageNumber: 1,
                pageSize: amount
            );

            //t => t.DueDate >= start && t.DueDate <= end && t.UserId == userId
            var filtered = tasks
                .Where(t => t.DueDate >= start && t.DueDate <= end)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt);

            //Act
            var result = await taskRepository.GetAll(sortFilterOptions).ToListAsync();

            //Assert
            CollectionAssert.AreEqual(filtered.Select(x => x.Id), result.Select(x => x.Id));
        }

        [Test]
        //amount, sortParam, statusParam, priorityParam, pageSize
        [TestCase(10000, SortTaskParams.DueDate, new int[] { 1, 2 }, new int[] { 1, 2 }, 100)]
        [TestCase(10000, SortTaskParams.DueDateDesc, new int[] { 0, 1 }, new int[] { 0, 1 }, 500)]
        [TestCase(10000, SortTaskParams.Priority, new int[] { 0, 2 }, new int[] { 0, 2 }, 1000)]
        [TestCase(10000, SortTaskParams.PriorityDesc, new int[] { 1, 2 }, new int[] { 1, 2 }, 2000)]
        public async Task GIVEN_Task_Repository_WHEN_I_apply_range_filters_single_filters_and_orderBy_THEN_tasks_are_filtered_correctly(int amount, 
            string sortParam, int[] statusParam, int[] priorityParam, int pageSize)
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var tasks = GetTestData(amount);
            var userId = tasks.First().UserId;
            SeedData(dbContext, tasks);

            var start = _fixture.Create<DateTime>();
            var end = start.AddDays(_fixture.Create<int>());

            var sortFilterOptions = new SortFilterTaskOptions(
                sort: sortParam,
                status: statusParam,
                priority: priorityParam,
                start: start,
                end: end,
                userId,
                pageNumber: 1,
                pageSize: pageSize
            );

            //query example
            //t => (t.DueDate >= start && t.DueDate <= end) && (t.Status == 1 || t.Status == 2) && (t.Priority == 1 || t.Priority == 2) && t.UserId == userId
            var status1 = Enum.Parse<TaskStatus>(statusParam[0].ToString());
            var status2 = Enum.Parse<TaskStatus>(statusParam[1].ToString());

            var priority1 = Enum.Parse<TaskPriority>(priorityParam[0].ToString());
            var priority2 = Enum.Parse<TaskPriority>(priorityParam[1].ToString());

            var filtered = tasks
                .Where(t => t.DueDate >= start && t.DueDate <= end)
                .Where(t => t.Status == status1 || t.Status == status2)
                .Where(t => t.Priority == priority1 || t.Priority == priority2)
                .Where(t => t.UserId == userId);

            filtered = sortParam switch
            {
                SortTaskParams.DueDate => filtered.OrderBy(t => t.DueDate),
                SortTaskParams.DueDateDesc => filtered.OrderByDescending(t => t.DueDate),
                SortTaskParams.Priority => filtered.OrderBy(t => t.Priority),
                SortTaskParams.PriorityDesc => filtered.OrderByDescending(t => t.Priority),
                _ => filtered.OrderByDescending(t => t.CreatedAt)
            };

            //Act
            var result = await taskRepository.GetAll(sortFilterOptions).ToListAsync();

            //Assert
            CollectionAssert.AreEqual(filtered.Select(x => x.Id), result.Select(x => x.Id));
        }

        [TearDown]
        public void TearDown()
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureDeleted();
        }
        private void SeedData(ApplicationDbContext context, IEnumerable<TaskEntity> data)
        {
            context.Tasks.AddRange(data);
            context.SaveChanges();
        }
        private List<TaskEntity> GetTestData(int amount)
        {
            var userId = Guid.NewGuid();
            var tasks = _fixture.Build<TaskEntity>()
                .With(t => t.DueDate, _fixture.Create<DateTime>())
                .With(t => t.Description, _fixture.Create<string>())
                .With(t => t.UserId, userId)
                .Without(t => t.User)
                .CreateMany(amount);

            return tasks.ToList();
        }
    }
}
