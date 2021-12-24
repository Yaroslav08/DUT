using Microsoft.Extensions.DependencyInjection;

namespace DUT.Infrastructure.IoC
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDUTServices(this IServiceCollection services)
        {



            return services;
        }
    }
}
