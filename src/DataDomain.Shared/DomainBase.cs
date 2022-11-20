using CQRS.Events.Shared;
using DataDomain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain.Shared
{
    public abstract class DomainBase<TEntity> : IDomainCommand<TEntity>, IDomainQuery<TEntity> where TEntity : IAggregateRoot
    {

        public DomainBase()
        {
        }

        public virtual ValueTask ApplyAsync(Guid aggregateRootId, params IEvent[] events)
        {
            return ApplyAsync(aggregateRootId, events.AsEnumerable());
        }

        public virtual async ValueTask ApplyAsync(Guid aggregateRootId, IEnumerable<IEvent> events)
        {
            await ApplyEventsAsync(aggregateRootId, events);
        }

        public abstract ValueTask ApplyEventsAsync(Guid aggregateRootId, IEnumerable<IEvent> events);
        public abstract Task<IEnumerable<TEntity>> ExecuteQueryAsync(IQueryBuilder<TEntity>? queryBuilder = null);
        public abstract Task<IEnumerable<TEntity>> ExecuteQueryAsync(Func<IQueryable<TEntity>, IEnumerable<TEntity>> queryFunction);

    }
}
