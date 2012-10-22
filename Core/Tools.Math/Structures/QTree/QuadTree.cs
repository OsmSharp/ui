// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Structures.QTree
{
    /// <summary>
    /// Interface to implement a factory to create a quad tree.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    public abstract class QuadTree<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// The root node of this quad tree.
        /// </summary>
        private QuadTreeNode<PointType> _root;

        /// <summary>
        /// Creates a new quad tree.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public QuadTree(
            int dept, GenericRectangleF2D<PointType> bounds)
        {
            _root = this.Create(dept, bounds);
        }

        /// <summary>
        /// Creates a new quad tree.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public QuadTree(
            int dept, double min_0, double min_1, double max_0, double max_1)
        {
            _root = this.Create(dept, min_0, min_1, max_0, max_1);
        }

        /// <summary>
        /// Creates a new quad tree node.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        protected internal QuadTreeNode<PointType> Create(
            int dept, GenericRectangleF2D<PointType> bounds)
        {
            return this.Create(dept, bounds.Min[0], bounds.Min[1], bounds.Max[0], bounds.Max[1]);
        }

        /// <summary>
        /// Creates a new quad tree node.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        protected internal abstract QuadTreeNode<PointType> Create(
            int dept, double min_0, double min_1, double max_0, double max_1);

        /// <summary>
        /// Expands the given quad tree nodes.
        /// </summary>
        /// <param name="min0"></param>
        /// <param name="min1"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        protected internal abstract QuadTreeNode<PointType> Expand(bool min0, bool min1,
            QuadTreeNode<PointType> node);

        /// <summary>
        /// Returns the smallest quadtree at depth zero.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public QuadTreeNode<PointType> this[PointType point]
        {
            get
            {
                if (point[0] < _root.Min0)
                { // expand towards the minimum.
                    // descide to the minimum or maximum.
                    if (point[1] < _root.Min1)
                    { // expand towards the minimum.
                        _root = this.Expand(true, true, _root);

                        // recursive call to the quad tree.
                        return this[point];
                    }
                    else
                    { // expand towards the maximum.
                        _root = this.Expand(true, false, _root);

                        // recursive call to the quad tree.
                        return this[point];
                    }
                }
                else if (point[0] > _root.Max0)
                { // expand towards the maximum.
                    // descide to the minimum or maximum.
                    if (point[1] < _root.Min1)
                    { // expand towards the minimum.
                        _root = this.Expand(false, true, _root);

                        // recursive call to the quad tree.
                        return this[point];
                    }
                    else
                    { // expand towards the maximum.
                        _root = this.Expand(false, false, _root);

                        // recursive call to the quad tree.
                        return this[point];
                    }
                }
                return _root[point];
            }
        }
    }
}
