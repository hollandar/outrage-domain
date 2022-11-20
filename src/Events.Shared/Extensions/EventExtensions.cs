namespace CQRS.Events.Shared.Extensions
{
    public static class EventExtensions
    {
        public static string GetEventName(this IEvent evt) {
            return evt.GetType().GetEventName();
        }
        
        public static string GetEventName(this Type eventType) {
            return eventType.Name;
        }
    }
}
