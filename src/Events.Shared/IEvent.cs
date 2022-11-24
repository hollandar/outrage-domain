namespace CQRS.Events.Shared
{
    public interface IEvent
    {
        Guid AggregateRootId { get; }
    }
}
