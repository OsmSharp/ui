using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.CH.Routing
{
    public class CHResolvedPoint : IResolvedPoint
    {
        public CHResolvedPoint(uint id, GeoCoordinate coordinate)
        {
            this.Id = id;
            this.Location = coordinate;
        }

        public string Name { get; set; }

        public uint Id { get; private set; }

        public GeoCoordinate Location { get; private set; }

        public List<KeyValuePair<string, string>> Tags { get; set; }
    }
}