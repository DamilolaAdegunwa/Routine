using BlockSms.Core.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace BlockSms.Core.EntityFrameworkCore
{
    public static class EfCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddEPTDbContext<TDbContext>(
            this IServiceCollection services,
            Action<IDbContextRegistrationOptionsBuilder> optionsBuilder = null)
            where TDbContext : EPTDbContext<TDbContext>
        {
            services.AddMemoryCache();

            var options = new DbContextRegistrationOptions(typeof(TDbContext), services);
            optionsBuilder?.Invoke(options);

            services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

            foreach (var dbContextType in options.ReplacedDbContextTypes)
            {
                services.Replace(ServiceDescriptor.Transient(dbContextType, typeof(TDbContext)));
            }

            new EfCoreRepositoryRegistrar(options).AddRepositories();

            return services;
        }
    }
}
