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

using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.PreProcessor;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Graphs.PreProcessing;
using OsmSharp.Routing.Osm.Interpreter;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Streams.Graphs
{
    /// <summary>
    /// A data processing target containing edges with the orignal OSM-tags and their original OSM-direction.
    /// </summary>
    public class LiveGraphOsmStreamTarget : DynamicGraphOsmStreamWriter<LiveEdge>
    {
        /// <summary>
        /// Holds the list of vehicle profiles to build routing information for.
        /// </summary>
        private HashSet<Vehicle> _vehicles;

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter">Inteprets the OSM-data.</param>
        /// <param name="tagsIndex">Holds all the tags.</param>
        public LiveGraphOsmStreamTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex)
            : this(dynamicGraph, interpreter, tagsIndex, new Dictionary<long, uint>())
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        public LiveGraphOsmStreamTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, IDictionary<long, uint> idTransformations)
            : this(dynamicGraph, interpreter, tagsIndex, idTransformations, null, null)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="box"></param>
        public LiveGraphOsmStreamTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, GeoCoordinateBox box)
            : this(dynamicGraph, interpreter, tagsIndex, new Dictionary<long, uint>(), box, null)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="vehicles"></param>
        /// <param name="interpreter">Inteprets the OSM-data.</param>
        /// <param name="tagsIndex">Holds all the tags.</param>
        public LiveGraphOsmStreamTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, IEnumerable<Vehicle> vehicles)
            : this(dynamicGraph, interpreter, tagsIndex, new Dictionary<long, uint>(), vehicles)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="vehicles"></param>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        public LiveGraphOsmStreamTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, IDictionary<long, uint> idTransformations, IEnumerable<Vehicle> vehicles)
            : this(dynamicGraph, interpreter, tagsIndex, idTransformations, null, vehicles)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="vehicles"></param>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="box"></param>
        public LiveGraphOsmStreamTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, GeoCoordinateBox box, IEnumerable<Vehicle> vehicles)
            : this(dynamicGraph, interpreter, tagsIndex, new Dictionary<long, uint>(), box, vehicles)
        {

        }

        /// <summary>
        /// Creates a new osm edge data processing target.
        /// </summary>
        /// <param name="dynamicGraph"></param>
        /// <param name="interpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="idTransformations"></param>
        /// <param name="box"></param>
        /// <param name="vehicles">The vehicle profiles to build routing information for.</param>
        public LiveGraphOsmStreamTarget(IDynamicGraphRouterDataSource<LiveEdge> dynamicGraph,
            IOsmRoutingInterpreter interpreter, ITagsCollectionIndex tagsIndex, IDictionary<long, uint> idTransformations, 
            GeoCoordinateBox box, IEnumerable<Vehicle> vehicles)
            : base(dynamicGraph, interpreter, null, tagsIndex, idTransformations, box)
        {
            _vehicles = new HashSet<Vehicle>();
            if (vehicles != null)
            {
                foreach (Vehicle vehicle in vehicles)
                {
                    _vehicles.Add(vehicle);
                }
            }
        }

        /// <summary>
        /// Adds an edge.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tags"></param>
        protected override bool AddRoadEdge(TagsCollectionBase tags, bool forward, uint from, uint to)
        {
            float latitude;
            float longitude;
            GeoCoordinate fromCoordinate = null;
            if (this.DynamicGraph.GetVertex(from, out latitude, out longitude))
            { // 
                fromCoordinate = new GeoCoordinate(latitude, longitude);
            }
            GeoCoordinate toCoordinate = null;
            if (this.DynamicGraph.GetVertex(to, out latitude, out longitude))
            { // 
                toCoordinate = new GeoCoordinate(latitude, longitude);
            }

            if (fromCoordinate != null && toCoordinate != null)
            { // calculate the edge data.
                LiveEdge edgeData = this.CalculateEdgeData(this.Interpreter.EdgeInterpreter, this.TagsIndex, tags, forward, fromCoordinate, toCoordinate);

                this.DynamicGraph.AddArc(from, to, edgeData, this.EdgeComparer);

                // add reverse edge and return true.
                LiveEdge reverseEdgeData = new LiveEdge()
                    {
                        Forward = !edgeData.Forward,
                        Tags = edgeData.Tags
                    };
                this.DynamicGraph.AddArc(to, from, reverseEdgeData, this.EdgeComparer);
            }
            return true;
        }

        /// <summary>
        /// Calculates edge data.
        /// </summary>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <param name="directionForward"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="edgeInterpreter"></param>
        /// <returns></returns>
        protected override LiveEdge CalculateEdgeData(IEdgeInterpreter edgeInterpreter, ITagsCollectionIndex tagsIndex, 
            TagsCollectionBase tags, bool directionForward, GeoCoordinate from, GeoCoordinate to)
        {
            if (edgeInterpreter == null) throw new ArgumentNullException("edgeInterpreter");
            if (tagsIndex == null) throw new ArgumentNullException("tagsIndex");
            if (tags == null) throw new ArgumentNullException("tags");
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            uint tagsId = tagsIndex.Add(tags);

            return new LiveEdge()
            {
                Forward = directionForward,
                Tags = tagsId
            };
        }

        /// <summary>
        /// Returns true if the edge is traversable.
        /// </summary>
        /// <param name="edgeInterpreter"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected override bool CalculateIsTraversable(IEdgeInterpreter edgeInterpreter,
            ITagsCollectionIndex tagsIndex, TagsCollectionBase tags)
        {
            if (_vehicles.Count > 0)
            { // limit only to vehicles in this list.
                foreach (Vehicle vehicle in _vehicles)
                {
                    if (vehicle.CanTraverse(tags))
                    { // one of them is enough.
                        return true;
                    }
                }
                return false;
            }
            return edgeInterpreter.IsRoutable(tags);
        }

        /// <summary>
        /// Returns the pre-processor.
        /// </summary>
        /// <returns></returns>
        public override IPreProcessor GetPreprocessor()
        {
            //return null;
            return new LiveEdgePreprocessor(this.DynamicGraph);
        }

        #region Static Processing Functions

        /// <summary>
        /// Preprocesses the data from the given OsmStreamReader and converts it directly to a routable data source.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<LiveEdge> Preprocess(OsmStreamSource reader,
                                                                        ITagsCollectionIndex tagsIndex,
                                                                        IOsmRoutingInterpreter interpreter)
        {
            var dynamicGraphRouterDataSource =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(dynamicGraphRouterDataSource, interpreter,
                tagsIndex);
            targetData.RegisterSource(reader);
            targetData.Pull();

            return dynamicGraphRouterDataSource;
        }

        /// <summary>
        /// Preprocesses the data from the given OsmStreamReader and converts it directly to a routable data source.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public static DynamicGraphRouterDataSource<LiveEdge> Preprocess(OsmStreamSource reader,
                                                                        IOsmRoutingInterpreter interpreter)
        {
            return LiveGraphOsmStreamTarget.Preprocess(reader, new TagsTableCollectionIndex(), interpreter);
        }

        #endregion
    }
}
