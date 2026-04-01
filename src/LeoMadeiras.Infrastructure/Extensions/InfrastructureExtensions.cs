
using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.Contracts.Services;
using LeoMadeiras.Infrastructure.Data;
using LeoMadeiras.Infrastructure.Data.Interceptors;
using LeoMadeiras.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeoMadeiras.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<AuditInterceptor>();

            services.AddDbContext<AppDbContext>((sp, opts) =>
                opts.UseSqlServer(config.GetConnectionString("Default"),
                        sql => sql.EnableRetryOnFailure())
                    .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IVendaRepository, VendaRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>(); 
            services.AddScoped<IJwtService, JwtService>();               

            return services;
        }
    }
}
