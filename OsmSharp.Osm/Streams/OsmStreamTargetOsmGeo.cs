//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2013 Abelshausen Ben
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
//using OsmSharp.Osm;
//using OsmSharp.Osm.Cache;
//using OsmSharp.Osm.Data.Streams.OsmGeoStream;

//namespace OsmSharp.Osm.Data.Streams
//{
//    /// <summary>
//    /// Osm stream writer that converts the stream to complete OsmGeo-objects.
//    /// </summary>
//    public abstract class OsmStreamTargetOsmGeo : OsmStreamTarget
//    {
//        /// <summary>
//        /// Holds the data cache.
//        /// </summary>
//        private readonly OsmDataCache _cache;

//        /// <summary>
//        /// Holds the target for the data.
//        /// </summary>
//        private IOsmGeoStreamTarget _target;

//        /// <summary>
//        /// Creates a new osm stream writer that converts the stream to complete OsmGeo-objects.
//        /// </summary>
//        protected OsmStreamTargetOsmGeo()
//        {
//            _cache = new OsmDataCacheMemory();
//        }

//        /// <summary>
//        /// Creates a new osm stream writer that converts the stream to complete OsmGeo-objects.
//        /// </summary>
//        protected OsmStreamTargetOsmGeo(OsmDataCache cache)
//        {
//            _cache = cache;
//        }

//        /// <summary>
//        /// Sets the target to send the resulting OsmGeo-objects to.
//        /// </summary>
//        /// <param name="target"></param>
//        public void RegisterOsmGeoTarget(IOsmGeoStreamTarget target)
//        {
//            _target = target;
//        }

//        #region OsmStreamWriter-Implementation

//        /// <summary>
//        /// Initializes this writer.
//        /// </summary>
//        public override void Initialize()
//        {
            
//        }

//        /// <summary>
//        /// Adds a new simple node.
//        /// </summary>
//        /// <param name="simpleNode"></param>
//        public override void AddNode(SimpleNode simpleNode)
//        {
//            _cache.AddNode(simpleNode);

//            _target.AddNode(Node.CreateFrom(simpleNode));
//        }

//        /// <summary>
//        /// Adds a new simple way.
//        /// </summary>
//        /// <param name="simpleWay"></param>
//        public override void AddWay(SimpleWay simpleWay)
//        {
//            _cache.AddWay(simpleWay);

//            Way way = Way.CreateFrom(simpleWay, _cache);
//            if (way != null)
//            {
//                _target.AddWay(way);
//            }
//        }

//        /// <summary>
//        /// Adds a new simple relation.
//        /// </summary>
//        /// <param name="simpleRelation"></param>
//        public override void AddRelation(Simple.SimpleRelation simpleRelation)
//        {
//            _cache.AddRelation(simpleRelation);

//            Relation relation = Relation.CreateFrom(simpleRelation, _cache);
//            if (relation != null)
//            {
//                _target.AddRelation(relation);
//            }
//        }

//        #endregion
//    }
//}