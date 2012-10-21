// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Structures.QTree;

namespace Osm.Data.Cache
{
    class DataSourceQuadTreeNode : QuadTreeNode<GeoCoordinate>
    {
        internal DataSourceQuadTreeNode(DataSourceQuadTree factory,
             int dept, GeoCoordinateBox bounds)
            :base(factory, dept, bounds)
        {

        }

        internal DataSourceQuadTreeNode(DataSourceQuadTree factory,
            int dept, double min_0, double min_1, double max_0, double max_1)
            :base(factory, dept, min_0, min_1, max_0, max_1)
        {

        }

        internal DataSourceQuadTreeNode(DataSourceQuadTree factory,
            bool min0, bool min1, QuadTreeNode<GeoCoordinate> node)
            :base(factory, min0, min1, node)
        {

        }
    }
}
