using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Persistence.EntityTypeConfiguration;

namespace TaskManagement.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TaskTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
