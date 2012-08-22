using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core
{
    /// <summary>
    /// Enumeration for the different types of osm data.
    /// </summary>
    public enum OsmType
    {
        Node,
        Way,
        Relation,
        ChangeSet
    }
}
