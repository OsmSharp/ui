using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Processor;
using Osm.Routing.CH.PreProcessing;
using Osm.Core.Simple;
using Osm.Routing.Core.Processor;
using Tools.Math.Geo;
using Osm.Routing.Core.Roads.Tags;
using Osm.Data.Core.DynamicGraph;

namespace Osm.Routing.CH.Processor
{
    /// <summary>
    /// Data processor target for osm data that will be converted into a Contracted Graph.
    /// </summary>
    public class CHDataProcessorTarget : DynamicGraphDataProcessorTarget<CHEdgeData>
    {
        /// <summary>
        /// Creates a new CH data processor target.
        /// </summary>
        /// <param name="target"></param>
        public CHDataProcessorTarget(IDynamicGraph<CHEdgeData> target)
            : base(target)
        {

        }

        /// <summary>
        /// Calculates edge data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        protected override CHEdgeData CalculateEdgeData(GeoCoordinate from, GeoCoordinate to,
            RoadTagsInterpreterBase interpreter)
        {
            CHEdgeData data = new CHEdgeData();
            data.Weight = (float) interpreter.Time(Core.VehicleEnum.Car, from, to);
            data.Forward = !interpreter.IsOneWayReverse();
            data.Backward = !interpreter.IsOneWay();

            return data;
        }
    }
}