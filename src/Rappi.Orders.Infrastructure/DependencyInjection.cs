using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rappi.Orders.Application.Interfaces;
using Rappi.Orders.Infrastructure.Persistence;
using Rappi.Orders.Infrastructure.Repository;

namespace Rappi.Orders.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("OrdersDb");
            services.AddDbContext<OrdersDbContext>(opt => opt.UseSqlServer(conn));

            services.AddScoped<IOrdersRepository, OrdersRepository>();

            return services;
        }
    }
}
