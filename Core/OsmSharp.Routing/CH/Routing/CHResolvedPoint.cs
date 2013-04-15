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
//using OsmSharp.Osm;
//using OsmSharp.Routing.Osm.Core.Resolving;

//namespace OsmSharp.Routing.CH.Routing
//{
//    public class CHResolvedPoint : IResolvedPoint
//    {
//        public CHResolvedPoint(uint id, GeoCoordinate coordinate)
//        {
//            this.Id = id;
//            this.Location = coordinate;
//            this.Tags = new List<KeyValuePair<string, string>>();
//        }

//        public string Name { get; set; }

//        public uint Id { get; private set; }

//        public GeoCoordinate Location { get; private set; }

//        public List<KeyValuePair<string, string>> Tags { get; private set; }
//    }
//}