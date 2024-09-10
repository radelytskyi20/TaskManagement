using TaskManagement.Domain.Contracts.Task;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Service.Implementations;
using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;
using TaskPriority = TaskManagement.Domain.Entities.TaskPriority;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Service.Interfaces;
using TaskManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Persistence.Extensions;
using TaskManagement.Domain.Contracts.Common;
using TaskManagement.Persistence;

namespace TaskManagement.Service.Tests
{
    [TestFixture]
    public class TaskManagerServiceTests
    {
        private IServiceProvider serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskManagerService, TaskManagerService>();

            serviceProvider = services.BuildServiceProvider();
        }

        [Test]
        public async Task GIVEN_Task_Manager_Service_WHEN_I_add_task_THEN_it_is_being_added_to_database()
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var taskManagerService = scope.ServiceProvider.GetRequiredService<ITaskManagerService>();

            var task = new TaskEntity
            {
                Title = "Test Task",
                Description = "Test Description",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low,
                DueDate = DateTime.Now.AddDays(1),
                UserId = Guid.NewGuid()
            };

            //Act
            var actualTaskId = await taskManagerService.CreateAsync(task.Title, task.Description, (int)task.Status, (int)task.Priority,
                task.DueDate, task.UserId);

            //Assert
            var getOneResult = await taskManagerService.GetOneAsync(actualTaskId, task.UserId);
            AssertPayloadResult(getOneResult);
            AssertsObjectsAreEqual(task.MapToDto(), getOneResult.Payload!);
        }

        [Test]
        public async Task GIVEN_Task_Manager_Service_WHEN_I_update_task_THEN_it_is_being_updated_in_database()
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var taskManagerService = scope.ServiceProvider.GetRequiredService<ITaskManagerService>();

            var task = new TaskEntity
            {
                Title = "Test Task",
                Description = "Test Description",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low,
                DueDate = DateTime.Now.AddDays(1),
                UserId = Guid.NewGuid()
            };

            //Act
            var taskId = await taskManagerService.CreateAsync(task.Title, task.Description, (int)task.Status,
                (int)task.Priority, task.DueDate, task.UserId);

            task.Title = "Updated Task";
            task.Description = "Updated Description";
            task.Status = TaskStatus.InProgress;
            task.Priority = TaskPriority.Medium;
            task.DueDate = DateTime.Now.AddDays(2);

            var updateResult = await taskManagerService.UpdateAsync(taskId, task.Title, task.Description, task.DueDate,
                (int)task.Status, (int)task.Priority, task.UserId);

            Assert.That(updateResult, Is.Not.Null);
            Assert.That(updateResult.Succeeded, Is.True);

            var getActualResult = await taskManagerService.GetOneAsync(taskId, task.UserId);
            AssertPayloadResult(getActualResult);
            AssertsObjectsAreEqual(task.MapToDto(), getActualResult.Payload!);
        }

        [Test]
        public async Task GIVEN_Task_Manager_Service_WHEN_I_delete_task_THEN_it_is_being_deleted_from_database()
        {
            //Arrange
            using var scope = serviceProvider.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var taskManagerService = scope.ServiceProvider.GetRequiredService<ITaskManagerService>();

            var task = new TaskEntity
            {
                Title = "Test Task",
                Description = "Test Description",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low,
                DueDate = DateTime.Now.AddDays(1),
                UserId = Guid.NewGuid()
            };

            //Act
            var taskId = await taskManagerService.CreateAsync(task.Title, task.Description, (int)task.Status,
                (int)task.Priority, task.DueDate, task.UserId);

            //check that task exists in database
            var getOneResult = await taskManagerService.GetOneAsync(taskId, task.UserId);
            AssertPayloadResult(getOneResult);

            var deleteResult = await taskManagerService.DeleteAsync(taskId, task.UserId);
            Assert.That(deleteResult, Is.Not.Null);
            Assert.That(deleteResult.Succeeded, Is.True);

            var getActualResult = await taskManagerService.GetOneAsync(taskId, task.UserId);
            Assert.That(getActualResult, Is.Not.Null);
            Assert.That(getActualResult.Succeeded, Is.False);
        }

        [TearDown]
        public void TearDown()
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureDeleted();
        }

        private void AssertsObjectsAreEqual(TaskDto expected, TaskDto actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.Title, Is.EqualTo(expected.Title));
                Assert.That(actual.Description, Is.EqualTo(expected.Description));
                Assert.That(actual.Status, Is.EqualTo(expected.Status));
                Assert.That(actual.Priority, Is.EqualTo(expected.Priority));
                Assert.That(actual.DueDate, Is.EqualTo(expected.DueDate));
                Assert.That(actual.CreatedAt, Is.EqualTo(expected.CreatedAt));
                Assert.That(actual.UpdatedAt, Is.EqualTo(expected.UpdatedAt));
            });
        }
        private void AssertPayloadResult<T>(PayloadResult<T>? result)
        {
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Payload, Is.Not.Null);
            });
        }
    }
}