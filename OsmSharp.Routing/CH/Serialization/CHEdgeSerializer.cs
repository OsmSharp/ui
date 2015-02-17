// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.IO;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Serialization;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Collections.Generic;

namespace OsmSharp.Routing.CH.Serialization
{
    /// <summary>
    /// Serializes/deserializes a graph.
    /// </summary>
    public class CHEdgeSerializer : GraphSerializer<CHEdgeData>
    {
        /// <summary>
        /// Returns the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "CHedgeFlatfile.v3.0"; }
        }

        /// <summary>
        /// Serializes the given graph and tags index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        protected override void DoSerialize(LimitedStream stream, DynamicGraphRouterDataSource<CHEdgeData> graph)
        {
            graph.Serialize(stream, CHEdgeData.SizeUints, CHEdgeData.MapFromDelegate, CHEdgeData.MapToDelegate);
        }

        /// <summary>
        /// Deserializes the given stream into a routable graph.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <param name="vehicles"></param>
        /// <returns></returns>
        protected override DynamicGraphRouterDataSource<CHEdgeData> DoDeserialize(LimitedStream stream, bool lazy, IEnumerable<string> vehicles)
        {
            return DynamicGraphRouterDataSource<CHEdgeData>.Deserialize(stream, CHEdgeData.SizeUints, CHEdgeData.MapFromDelegate, CHEdgeData.MapToDelegate);
        }
    }
}