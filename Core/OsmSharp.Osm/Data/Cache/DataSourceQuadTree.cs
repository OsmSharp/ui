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
//using OsmSharp.Tools.Math.Structures.QTree;

//namespace OsmSharp.Osm.Data.Cache
//{
//    public class DataSourceQuadTree : QuadTree<GeoCoordinate>
//    {
//        /// <summary>
//        /// Creates a new quad tree.
//        /// </summary>
//        /// <param name="bounds"></param>
//        /// <returns></returns>
//        public DataSourceQuadTree(
//            int dept, GeoCoordinateBox bounds)
//            :base(dept, bounds)
//        {

//        }

//        /// <summary>
//        /// Creates a new quad tree.
//        /// </summary>
//        /// <param name="bounds"></param>
//        /// <returns></returns>
//        public DataSourceQuadTree(
//            int dept, double min_0, double min_1, double max_0, double max_1)
//            :base(dept, min_0, min_1, max_0, max_1)
//        {

//        }

//        /// <summary>
//        /// Creates a new quad tree node.
//        /// </summary>
//        /// <param name="dept"></param>
//        /// <param name="min_0"></param>
//        /// <param name="min_1"></param>
//        /// <param name="max_0"></param>
//        /// <param name="max_1"></param>
//        /// <returns></returns>
//        protected override QuadTreeNode<GeoCoordinate> Create(
//            int dept, double min_0, double min_1, double max_0, double max_1)
//        {
//            return new DataSourceQuadTreeNode(this, dept, min_0, min_1, max_0, max_1);
//        }

//        /// <summary>
//        /// Expands the given node in the given directions.
//        /// </summary>
//        /// <param name="min0"></param>
//        /// <param name="min1"></param>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        protected override QuadTreeNode<GeoCoordinate> Expand(bool min0, bool min1, QuadTreeNode<GeoCoordinate> node)
//        {
//            return new DataSourceQuadTreeNode(this, min0, min1, node);
//        }
//    }
//}
