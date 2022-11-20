using System.Runtime.Serialization;

namespace CQRS.Events.Shared
{
    [Serializable]
    internal class AggregateRootIdFormatException : Exception
    {
        public AggregateRootIdFormatException()
        {
        }

        public AggregateRootIdFormatException(string? message) : base(message)
        {
        }

        public AggregateRootIdFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AggregateRootIdFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}