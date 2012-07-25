using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Structures.QTree;

namespace Osm.Data.Cache
{
    public class DataSourceQuadTree : QuadTree<GeoCoordinate>
    {
        /// <summary>
        /// Creates a new quad tree.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public DataSourceQuadTree(
            int dept, GeoCoordinateBox bounds)
            :base(dept, bounds)
        {

        }

        /// <summary>
        /// Creates a new quad tree.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public DataSourceQuadTree(
            int dept, double min_0, double min_1, double max_0, double max_1)
            :base(dept, min_0, min_1, max_0, max_1)
        {

        }

        /// <summary>
        /// Creates a new quad tree node.
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="min_0"></param>
        /// <param name="min_1"></param>
        /// <param name="max_0"></param>
        /// <param name="max_1"></param>
        /// <returns></returns>
        protected override QuadTreeNode<GeoCoordinate> Create(
            int dept, double min_0, double min_1, double max_0, double max_1)
        {
            return new DataSourceQuadTreeNode(this, dept, min_0, min_1, max_0, max_1);
        }

        /// <summary>
        /// Expands the given node in the given directions.
        /// </summary>
        /// <param name="min0"></param>
        /// <param name="min1"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override QuadTreeNode<GeoCoordinate> Expand(bool min0, bool min1, QuadTreeNode<GeoCoordinate> node)
        {
            return new DataSourceQuadTreeNode(this, min0, min1, node);
        }
    }
}
