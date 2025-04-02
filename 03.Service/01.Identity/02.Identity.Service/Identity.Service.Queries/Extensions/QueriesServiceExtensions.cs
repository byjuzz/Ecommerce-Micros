using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Service.Queries.Extensions
{
    public static class QueriesServiceExtensions
    {
        public static IServiceCollection RegisterQueryHandlers(this IServiceCollection services)
        {
            return services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(QueriesServiceExtensions).Assembly));
        }
    }
}
