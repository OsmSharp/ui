using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Simple
{
    /// <summary>
    /// Primive used as a base class for any osm object that has a meaning on the map (Nodes, Ways and Relations).
    /// </summary>
    public class SimpleOsmGeo
    {
        public long? Id { get; set; }

        public SimpleOsmGeoType Type { get; protected set; }

        public IDictionary<string,string> Tags { get; set; }

        public long? ChangeSetId { get; set; }

        public bool? Visible { get; set; }

        public DateTime? TimeStamp { get; set; }

        public ulong? Version { get; set; }

        public long? UserId { get; set; }

        public string UserName { get; set; }
    }
}
