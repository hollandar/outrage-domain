using CQRS.Events.Shared.Extensions;
using System.Reflection;

namespace CQRS.Events.Shared
{
    public class EventLibraryService
    {
        private readonly IDictionary<string, Type> eventTypes = new Dictionary<string, Type>();
        public EventLibraryService(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsAssignableTo(typeof(IEvent)) && type.IsClass)
                {
                    eventTypes.Add(type.GetEventName(), type);
                }
            }
        }

        public bool TryGetType(string name, out Type? type)
        {
            return eventTypes.TryGetValue(name, out type);
        }
    }
}
