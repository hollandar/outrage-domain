using CQRS.Events.Shared;
using DataDomain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain.EFCore
{
    public class EntityDomain<TEntity> : EventSourcingDomainBase<TEntity>
        where TEntity : class, IAggregateRoot, new()
    {
        private readonly IEntityBuilder entityBuilder;
        private readonly IAggregateRootBuilder<TEntity> changeBuilder;

        public EntityDomain(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.entityBuilder = serviceProvider.GetRequiredService<IEntityBuilder>();
            this.changeBuilder = serviceProvider.GetRequiredService<IAggregateRootBuilder<TEntity>>();
        }

        public override async ValueTask ApplyEventsAsync(Guid aggregateRootId, IEnumerable<IEvent> events)
        {
            var product = this.entityBuilder.GetQueryable<TEntity>().FirstOrDefault(r => r.Id == aggregateRootId);

            foreach (var @event in events)
            {
                await changeBuilder.ApplyAsync(product, @event);
            }

            await this.entityBuilder.SaveChangesAsync();
        }
        public override Task<IEnumerable<TEntity>> ExecuteQueryAsync(IQueryBuilder<TEntity>? queryBuilder = null)
        {
            var entities = this.entityBuilder.GetQueryable<TEntity>();
            if (queryBuilder == null)
                return Task.FromResult(entities.AsEnumerable());
            else
                return Task.FromResult(queryBuilder.BuildQuery(entities));
        }

        public override Task<IEnumerable<TEntity>> ExecuteQueryAsync(Func<IQueryable<TEntity>, IEnumerable<TEntity>> queryFunction)
        {
            var entities = this.entityBuilder.GetQueryable<TEntity>();
            return Task.FromResult(queryFunction(entities));
        }
    }
}
