// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.PreProcessor;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Test.Unittests.Routing
{
    /// <summary>
    /// A reference stream target to build a reference network.
    /// </summary>
    public class ReferenceGraphTarget : LiveGraphOsmStreamTarget
    {
        /// <summary>
        /// Creates a reference stream target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicles"></param>
        public ReferenceGraphTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph, IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, IEnumerable<Vehicle> vehicles)
            : base(dynamicGraph, interpreter, tagsIndex, new HugeDictionary<long, uint>(), null, vehicles, false)
        {

        }

        /// <summary>
        /// Returns the preprocessor.
        /// </summary>
        /// <returns></returns>
        public override IPreProcessor GetPreprocessor()
        {
            return null;
        }
    }
}
