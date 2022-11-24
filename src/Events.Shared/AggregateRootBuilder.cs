using CQRS.Events.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Events.Shared
{
    public interface IAggregateRootBuilder<TEntity> where TEntity : class, IAggregateRoot, new()
    {
        Task ApplyAsync(TEntity? entity, IEvent eventRecord);
    }

    public abstract class AggregateRootBuilder<TEntity> : IAggregateRootBuilder<TEntity> where TEntity : class, IAggregateRoot, new()
    {
        private readonly IEntityBuilder entityBuilder;
        private readonly List<IEvent> events = new();

        public AggregateRootBuilder(
            IEntityBuilder entityBuilder)
        {
            this.entityBuilder = entityBuilder;
        }

        public IEnumerable<IEvent> Events { get { return events.AsReadOnly(); } }
        public IEntityBuilder EntityBuilder => entityBuilder;       
        
        public Task ApplyAsync(TEntity? entity, IEvent eventRecord)
        {
            this.events.Add(eventRecord);
            return ApplyEventAsync(entity, eventRecord);
        }

        /// <summary>
        /// Final implentation of apply should be able to impose all events,
        /// throw an exception if an event is unknown
        /// </summary>
        /// <param name="eventRecord">Event to apply</param>
        /// <returns></returns>
        protected abstract Task ApplyEventAsync(TEntity? entity, IEvent eventRecord);
    }
}
