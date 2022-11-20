using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.EventStorage.Shared
{
    public readonly struct EventNumber: IEquatable<EventNumber>, IComparable<EventNumber>
    {
        readonly ulong position;

        public EventNumber(ulong position)
        {
            this.position = position;
        }

        public ulong Position => position;

        public static EventNumber Start = new EventNumber(0);
        public static EventNumber End = new EventNumber(ulong.MaxValue);

        public bool Equals(EventNumber other)
        {
            return this.position == other.position;
        }

        public int CompareTo(EventNumber other)
        {
            return this.position.CompareTo(other.position);
        }
    }
}
