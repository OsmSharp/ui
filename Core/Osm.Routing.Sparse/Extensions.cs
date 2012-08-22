using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Core.Sparse;
using Osm.Routing.Core.Roads.Tags;
using Tools.Math.Geo;
using Osm.Routing.Core;

namespace Osm.Routing.Sparse
{
    public static class Extensions
    {
        public static void ReCalculateWeight(this SparseVertex from, long to_id, ISparseData data)
        {
            SparseVertexNeighbour neighbour = from.GetSparseVertexNeighbour(to_id);

            // calculate weight.
            double weight = 0;
            RoadTagsInterpreterBase tags_interpreter = new RoadTagsInterpreterBase(
                neighbour.Tags);
            GeoCoordinate from_location = from.Location;
            GeoCoordinate to_location = null;
            SimpleVertex simple;
            for (int idx = 0; idx < neighbour.Nodes.Length; idx++)
            {
                simple = data.GetSimpleVertex(neighbour.Nodes[idx]);
                to_location = new GeoCoordinate(simple.Latitude, simple.Longitude);
                weight = weight + tags_interpreter.Time(VehicleEnum.Car, from_location, to_location);
                from_location = to_location;
            }
            simple = data.GetSimpleVertex(to_id);
            to_location = new GeoCoordinate(simple.Latitude, simple.Longitude);
            weight = weight + tags_interpreter.Time(VehicleEnum.Car, from_location, to_location);

            neighbour.Weight = weight;
        }
    }
}
