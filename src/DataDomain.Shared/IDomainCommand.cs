using CQRS.Events.Shared;
using System.ComponentModel.Design;

namespace DataDomain.Shared
{
    public interface IDomainCommand<TEntity> where TEntity: IAggregateRoot
    {
        ValueTask ApplyAsync(Guid aggregateRootId, IEnumerable<IEvent> events);
        ValueTask ApplyAsync(Guid aggregateRootId, params IEvent[] events);
    }
}