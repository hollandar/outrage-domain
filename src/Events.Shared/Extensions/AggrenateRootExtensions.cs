using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Events.Shared.Extensions
{
    public static class AggrenateRootExtensions
    {
        public static AggregateRootId GetAggregateRootId(this IAggregateRoot aggregateRoot, string prefix)
        {
            var aggregateName = aggregateRoot.GetType().Name;
            return new AggregateRootId(prefix, aggregateName, aggregateRoot.Id);
        }

        public static AggregateRootId GetAggregateRootId(this Type aggregateRootType, Guid id, string prefix)
        {
            if (!aggregateRootType.IsAssignableTo(typeof(IAggregateRoot)))
                throw new ArgumentException($"{aggregateRootType.FullName} does not implement IAggregateRoot.");
            var aggregateName = aggregateRootType.Name;
            return new AggregateRootId(prefix, aggregateName, id);
        }
    }
}
