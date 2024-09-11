using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Domain.Constants.Data;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Persistence.Helpers;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Persistence.Repositories;

namespace TaskManagement.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(ConnectionNames.DbConnection));
            });

            //Helpers
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IJwtProvider, JwtProvider>();
            services.AddTransient<IPasswordValidator, PasswordValidator>();

            //Repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();

            return services;
        }
    }
}
