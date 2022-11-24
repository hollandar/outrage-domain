using CQRS.Events.Shared;
using DataDomain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DataDomain.Web
{
    public static class StartupExtensions
    {
        public static IEndpointConventionBuilder MapEventPost<TEvent, THandler>(this WebApplication webApplication, string route, Func<TEvent, THandler, Task<IResult>> handle) 
            where TEvent: IEvent
        {
            return webApplication.MapPost(route, async ([FromBody] TEvent @event, THandler handler) => {
                return await handle(@event, handler);
            });
        }
    }
}