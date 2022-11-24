using CQRS.Events.Shared;
using DataDomain.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain.Web
{
    public abstract class DomainEventHandler<TAggregateRoot, TEvent> 
        where TAggregateRoot: IAggregateRoot 
        where TEvent: IEvent
    {
        IDomainCommand<TAggregateRoot> eventHandler;
        public DomainEventHandler(IServiceProvider serviceProvider)
        {
            this.eventHandler = serviceProvider.GetRequiredService<IDomainCommand<TAggregateRoot>>();
            
        }

        protected IDomainCommand<TAggregateRoot> Command => eventHandler;
    }
}
