using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Service.Implementations;
using TaskManagement.Service.Interfaces;

namespace TaskManagement.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<ITaskManagerService, TaskManagerService>();

            return services;
        }
    }
}
