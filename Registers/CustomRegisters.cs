using Microsoft.Extensions.DependencyInjection;
using msm_ws.Repositories;

namespace msm_ws.Registers
{
    public static class CustomRegisters
    {
        /// <summary>
        /// Registro de los servicios/repositorios personalizados
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection addCustomRegisters(this IServiceCollection services)
        {
            services.AddTransient(typeof(MainRepository));
            services.AddTransient(typeof(LotesRepository));
            return services;
        }
    }
}
