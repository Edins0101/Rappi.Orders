using Microsoft.Extensions.DependencyInjection;
using Rappi.Orders.Application.UseCases;

namespace Rappi.Orders.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOrdersAppService, OrdersAppService>();
            return services;
        }
    }
}
