using CQRS.Events.Shared;
using DataDomain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain.EFCore
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddEntityBuilder<TDbContext>(this IServiceCollection services) 
            where TDbContext : DbContext
        {
            services.AddScoped<IEntityBuilder, DbContextEntityBuilder<TDbContext>>();

            return services;
        }

        public static IServiceCollection AddDomainObject<TEntity, TEntityBuilder>(this IServiceCollection services) 
            where TEntity: class, IAggregateRoot, new()
            where TEntityBuilder: class, IAggregateRootBuilder<TEntity>
        {
            services.AddScoped<IAggregateRootBuilder<TEntity>, TEntityBuilder>();
            services.AddScoped<IDomainCommand<TEntity>, EntityDomain<TEntity>>();
            services.AddScoped<IDomainQuery<TEntity>, EntityDomain<TEntity>>();

            return services;
        }
    }
}
