using CQRS.Events.Shared;
using System.ComponentModel.Design;

namespace DataDomain.Shared
{
    public interface IDomainCommand<TEntity> where TEntity: IAggregateRoot
    {
        ValueTask ApplyAsync(IEnumerable<IEvent> events);
        ValueTask ApplyAsync(params IEvent[] events);
    }
}