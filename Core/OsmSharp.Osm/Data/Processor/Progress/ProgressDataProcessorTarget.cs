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
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Osm.Simple;

//namespace OsmSharp.Osm.Data.Core.Processor.Progress
//{
//    public class ProgressDataProcessorTarget : DataProcessorTarget
//    {
//        private DataProcessorTarget _target;

//        private long _start;
//        private long _node;
//        private long _way;
//        private long _relation;

//        private double _max_latitude = double.MinValue;
//        private double _max_longitude = double.MinValue;
//        private double _min_latitude = double.MaxValue;
//        private double _min_longitude = double.MaxValue;

//        public ProgressDataProcessorTarget(DataProcessorTarget target)
//        {
//            _target = target;
//        }

//        public override void Initialize()
//        {
//            _target.Initialize();

//            _start = DateTime.Now.Ticks;
//            _node = 0;
//            _way = 0;
//            _relation = 0;
//        }

//        public override void ApplyChange(SimpleChangeSet change)
//        {
//            _target.ApplyChange(change);
//        }

//        public override void AddNode(SimpleNode node)
//        {
//            _target.AddNode(node);

//            _node++;

//            if ((_node % 10000) == 0)
//            {
//                long stop = DateTime.Now.Ticks;
//                long seconds = (stop - _start) / TimeSpan.TicksPerSecond;
//                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Node[{0}]: {1}node/s", _node, (int)((double)_node / (double)seconds));
//            }

//            if (node.Latitude > _max_latitude)
//            {
//                _max_latitude = node.Latitude.Value;
//            }
//            if (node.Latitude < _min_latitude)
//            {
//                _min_latitude = node.Latitude.Value;
//            }
//            if (node.Longitude > _max_longitude)
//            {
//                _max_longitude = node.Longitude.Value;
//            }
//            if (node.Longitude < _min_longitude)
//            {
//                _min_longitude = node.Longitude.Value;
//            }
//        }

//        public override void AddWay(SimpleWay way)
//        {
//            _target.AddWay(way);

//            _way++;

//            if ((_way % 1000) == 0)
//            {
//                long stop = DateTime.Now.Ticks;
//                long seconds = (stop - _start) / TimeSpan.TicksPerSecond;
//                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Way[{0}]: {1}ways/s", _way, (int)((double)_way / (double)seconds));
//            }
//        }

//        public override void AddRelation(SimpleRelation relation)
//        {
//            _target.AddRelation(relation);

//            _relation++;

//            if ((_relation % 1000) == 0)
//            {
//                long stop = DateTime.Now.Ticks;
//                long seconds = (stop - _start) / TimeSpan.TicksPerSecond;
//                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Relation[{0}]: {1}relation/s", _relation, (int)((double)_relation / (double)seconds));
//            }
//        }

//        public GeoCoordinate Center
//        {
//            get
//            {
//                return (new GeoCoordinateBox(new GeoCoordinate(_max_latitude, _max_longitude),
//                    new GeoCoordinate(_min_latitude, _min_longitude)).Center);
//            }
//        }

//        public override void Close()
//        {
//            _target.Close();
//        }
//    }
//}
