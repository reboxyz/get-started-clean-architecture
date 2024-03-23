
using GymManagement.Application.Common.Interfaces;
using GymManagement.Infrastructure.Common.Persistence;
using GymManagement.Infrastructure.Subscriptions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMediatR(options => {
            options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
        });

        services.AddDbContext<GymManagementDbContext>(options => 
            options.UseSqlite("Data Source = GymManagement.db")
        );
        services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        // Note! IUnitOfWork is the DBContext!
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<GymManagementDbContext>());
        
        return services;
    }
}
