namespace CQRS.Events.Shared
{
    public interface IAggregateRoot
    {
        Guid Id { get; set; }
        ulong Version { get; set; }
    }
}
