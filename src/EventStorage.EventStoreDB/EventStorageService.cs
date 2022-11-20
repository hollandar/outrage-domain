using CQRS.Events.Shared;
using CQRS.Events.Shared.Extensions;
using CQRS.EventStorage.Shared;
using EventStorage.EventStoreDB.Options;
using EventStorage.Shared;
using EventStore.Client;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CQRS.EventStorage.EventStoreDB
{
    public class EventStoreDBStorageService : IEventStorageService
    {
        private readonly EventStoreClient client;

        public EventStoreDBStorageService(IOptions<EventStoreDBOptions> eventStoreOptions)
        {
            var settings = EventStore.Client.EventStoreClientSettings.Create(eventStoreOptions.Value.ConnectionString);
            client = new EventStore.Client.EventStoreClient(settings);
        }

        public async Task StoreEventsAsync(AggregateRootId aggregateRootId, params IEvent[] events)
        {
            await StoreEventsAsync(aggregateRootId, events);
        }

        public async Task StoreEventsAsync(AggregateRootId aggregateRootId, IEnumerable<IEvent> events)
        {
            List<EventData> eventDatas = new();
            foreach (var evt in events)
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(evt, evt.GetType());
                var eventData = new EventData(
                    Uuid.NewUuid(),
                    evt.GetEventName(),
                    bytes
                );

                eventDatas.Add(eventData);
            }
         
            await client.AppendToStreamAsync(
                aggregateRootId.ToString(),
                StreamState.Any,
                eventDatas
            );

        }

        public async IAsyncEnumerable<IEvent> ReadEventsAsync(AggregateRootId aggregateRootId, EventNumber eventNumber, EventLibraryService eventLibrary)
        {
            var evtsResult = client.ReadStreamAsync(Direction.Forwards, aggregateRootId.ToString(), new StreamPosition(eventNumber.Position));

            await foreach (var evt in evtsResult)
            {
                var eventTypeName = evt.Event.EventType;
                Type? eventType;
                if (eventLibrary.TryGetType(eventTypeName, out eventType) && eventType != null)
                {
                    var evtRecord = (IEvent?)JsonSerializer.Deserialize(evt.Event.Data.Span, eventType);
                    if (evtRecord != null)
                        yield return evtRecord;
                }
            }

        }
    }
}
