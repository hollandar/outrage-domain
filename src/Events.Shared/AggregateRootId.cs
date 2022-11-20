using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CQRS.Events.Shared
{
    partial class AggregateRootIdRegex
    {
        [GeneratedRegex("(?<prefix>.+?)-(?<name>.+?)-(?<id>.+)")]
        public static partial Regex AggregateMatcher();
    }

    public sealed class AggregateRootId
    {
        private readonly string prefix;
        private readonly string aggregateName;
        private readonly Guid id;

        public AggregateRootId(string prefix, string aggregateName, Guid id)
        {
            this.prefix = prefix;
            this.aggregateName = aggregateName;
            this.id = id;
        }

        private AggregateRootId(string aggregateRootId)
        {
            var match = AggregateRootIdRegex.AggregateMatcher().Match(aggregateRootId);
            if (match.Success)
            {
                var prefix = match.Groups["prefix"].Value;
                var name = match.Groups["name"].Value;
                var idString = match.Groups["id"].Value;
                Guid id;
                if (!Guid.TryParse(idString, out id))
                {
                    throw new AggregateRootIdFormatException($"{aggregateRootId} does not include valid guid for id in format 'prefix-name-id'.");
                }

                this.prefix = prefix;
                this.aggregateName = name;
                this.id = id;
            }

            throw new AggregateRootIdFormatException($"{aggregateRootId} did not match the format 'prefix-name-id'.");
        }

        public override string ToString()
        {
            var idBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(prefix))
                idBuilder.Append(prefix).Append("-");
            idBuilder.Append(aggregateName).Append("-").Append(id);
            return idBuilder.ToString();
        }

        public static AggregateRootId FromString(string aggregateRootId)
        {
            return new AggregateRootId(aggregateRootId);
        }


    }
}
