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
