using CQRS.Events.Shared;

namespace CQRS.EventStorage.Shared
{
    public interface IEventStorageService
    {
        IAsyncEnumerable<IEvent> ReadEventsAsync(AggregateRootId aggregateRootId, EventNumber eventNumber, EventLibraryService eventLibrary);
        Task StoreEventsAsync(AggregateRootId aggregateRootId, params IEvent[] events);
        Task StoreEventsAsync(AggregateRootId aggregateRootId, IEnumerable<IEvent> events);
    }
}
