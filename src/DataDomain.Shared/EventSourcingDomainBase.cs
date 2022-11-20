using CQRS.Events.Shared;
using CQRS.Events.Shared.Extensions;
using CQRS.EventStorage.Shared;
using DataDomain.Shared;
using Events.Shared;
using EventStorage.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace ShoppingCart.Data
{
    public abstract class EventSourcingDomainBase<TEntity> : DomainBase<TEntity> where TEntity : IAggregateRoot
    {
        private readonly IEventStorageService? eventStorageService;
        private readonly EventSourcePrefix? eventSourcePrefix;

        public EventSourcingDomainBase(IServiceProvider serviceProvider) : base()
        {
            this.eventStorageService = serviceProvider.GetService<IEventStorageService>();
            this.eventSourcePrefix = serviceProvider.GetService<EventSourcePrefix>();
        }

        public override async ValueTask ApplyAsync(Guid aggregateRootId, IEnumerable<IEvent> events)
        {
            await ApplyEventsAsync(aggregateRootId, events);

            if (typeof(TEntity).IsAssignableTo(typeof(IEventSourcing)) && eventStorageService != null)
            {
                var eventSourcePrefix = this.eventSourcePrefix?.Prefix ?? String.Empty;
                var aggregateRootIdObject = typeof(TEntity).GetAggregateRootId(aggregateRootId, eventSourcePrefix);
                await eventStorageService.StoreEventsAsync(aggregateRootIdObject, events);
            }
        }
    }
}
