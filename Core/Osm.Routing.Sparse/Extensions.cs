// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
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
