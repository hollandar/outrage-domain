using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStorage.Shared
{
    public class EventSourcePrefix
    {
        public EventSourcePrefix(string prefix)
        {
            this.Prefix = prefix;
        }

        public string Prefix { get; set; }
    }
}
