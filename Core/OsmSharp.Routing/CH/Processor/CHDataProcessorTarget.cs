//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Osm.Data.Core.Processor;
//using OsmSharp.Routing.CH.PreProcessing;
//using OsmSharp.Osm.Simple;
//using OsmSharp.Routing.Osm.Core.Processor;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Routing.Osm.Core.Roads.Tags;
//using OsmSharp.Osm.Data.Core.DynamicGraph;
//using OsmSharp.Routing.CH.PreProcessing.Tags;
//using OsmSharp.Routing.CH.Routing;

//namespace OsmSharp.Routing.CH.Processor
//{
//    /// <summary>
//    /// Data processor target for osm data that will be converted into a Contracted Graph.
//    /// </summary>
//    public class CHDataProcessorTarget : DynamicGraphDataProcessorTarget<CHEdgeData>
//    {
//        /// <summary>
//        /// Creates a new tags index.
//        /// </summary>
//        private CHDataSource _data_source;

//        /// <summary>
//        /// Creates a new CH data processor target.
//        /// </summary>
//        /// <param name="target"></param>
//        public CHDataProcessorTarget(CHDataSource data_source)
//            : base(data_source.Graph)
//        {
//            _data_source = data_source;
//        }

//        /// <summary>
//        /// Calculates edge data.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <param name="interpreter"></param>
//        /// <returns></returns>
//        protected override CHEdgeData CalculateEdgeData(GeoCoordinate from, GeoCoordinate to,
//            RoadTagsInterpreterBase interpreter)
//        {
//            CHEdgeData data = new CHEdgeData();
//            data.Weight = (float) interpreter.Time(Core.VehicleEnum.Car, from, to);
//            data.Forward = !interpreter.IsOneWayReverse();
//            data.Backward = !interpreter.IsOneWay();
//            if (interpreter.Tags != null && interpreter.Tags.Count > 0)
//            {
//                data.HasTags = true;
//                data.Tags = _data_source.Add(interpreter.Tags);
//            }
//            else
//            {
//                data.HasTags = false;
//            }
//            data.HasContractedVertex = false;

//            return data;
//        }
//    }
//}