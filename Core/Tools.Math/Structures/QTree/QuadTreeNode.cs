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
    /// An implementation of a quad tree.
    /// </summary>
    public abstract class QuadTreeNode<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// The bottom left child quad tree.
        /// </summary>
        private QuadTreeNode<PointType> _min_min;

        /// <summary>
        /// The bottom right child quad tree.
        /// </summary>
        private QuadTreeNode<PointType> _min_max;

        /// <summary>
        /// The top right child quad tree.
        /// </summary>
        private QuadTreeNode<PointType> _max_min;

        /// <summary>
        /// The top left child quad tree.
        /// </summary>
        private QuadTreeNode<PointType> _max_max;

        /// <summary>
        /// The min of this quad tree in dimension 0.
        /// </summary>
        private double _min_0;

        /// <summary>
        /// The max of this quad tree in dimension 0.
        /// </summary>
        private double _max_0;

        /// <summary>
        /// The min of this quad tree in dimension 1.
        /// </summary>
        private double _min_1;

        /// <summary>
        /// The max of this quad tree in dimension 1.
        /// </summary>
        private double _max_1;

        /// <summary>
        /// The middle of this level of the tree in dimension 0.
        /// </summary>
        private double _middle_0;

        /// <summary>
        /// The middle of this level of the tree in dimension 1.
        /// </summary>
        private double _middle_1;

        /// <summary>
        /// The dept of this quad tree.
        /// </summary>
        private int _depth;

        /// <summary>
        /// The factory to create the children.
        /// </summary>
        private QuadTree<PointType> _factory;

        /// <summary>
        /// Creates a quad tree with given bounds.
        /// </summary>
        /// <param name="box"></param>
        protected QuadTreeNode(QuadTree<PointType> factory, int dept, GenericRectangleF2D<PointType> bounds)
        {
            _factory = factory;
            _depth = dept;

            _max_0 = bounds.Max[0]; // max x = right.
            _max_1 = bounds.Max[1]; // max y = top.

            _min_0 = bounds.Min[0]; // min y = bottom.
            _min_1 = bounds.Min[1]; // min x = left.

            // calculate the middles.
            _middle_0 = (_min_0 + _max_0) / 2.0;
            _middle_1 = (_min_1 + _max_1) / 2.0;
        }

        /// <summary>
        /// Creates a quad tree with given bounds.
        /// </summary>
        /// <param name="box"></param>
        protected QuadTreeNode(QuadTree<PointType> factory, int dept, double min_0, double min_1, double max_0, double max_1)
        {
            _factory = factory;
            _depth = dept;

            _max_1 = max_1; // max y = top.
            _max_0 = max_0; // max x = right.

            _min_0 = min_0; // min y = bottom.
            _min_1 = min_1; // min x = left.

            // calculate the middles.
            _middle_0 = (_min_0 + _max_0) / 2.0;
            _middle_1 = (_min_1 + _max_1) / 2.0;
        }

        protected QuadTreeNode(QuadTree<PointType> factory, bool min0, bool min1,
            QuadTreeNode<PointType> node)
        {
            _factory = factory;
            _depth = node.Dept + 1;

            double diff_0 = node.Max0 - node.Min0;
            double diff_1 = node.Max1 - node.Min1;

            if (min0)
            {
                _min_0 = node.Min0 - diff_0;
                _max_0 = node.Max0;
            }
            else
            {
                _min_0 = node.Min0;
                _max_0 = node.Max0 + diff_0;
            }

            if (min1)
            {
                _min_1 = node.Min1 - diff_1;
                _max_1 = node.Max1;
            }
            else
            {
                _min_1 = node.Min1;
                _max_1 = node.Max1 + diff_1;
            }

            // calculate the middles.
            _middle_0 = (_min_0 + _max_0) / 2.0;
            _middle_1 = (_min_1 + _max_1) / 2.0;
        }

        /// <summary>
        /// Returns the smallest quadtree at depth zero.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public QuadTreeNode<PointType> this[PointType point]
        {
            get
            {
                // return this tree if this one is at dimension 0.
                if (_depth == 0)
                {
                    return this;
                }

                if (_middle_0 < point[0])
                { // small side of dimension 0.
                    if (_middle_1 < point[1])
                    { // small side of dimension 1.
                        if (_min_min == null)
                        {
                            _min_min = _factory.Create(_depth - 1,
                                _min_0, _min_1, _middle_0, _middle_1);
                        }
                        return _min_min[point];
                    }
                    else
                    { // large side of dimension 1.
                        if (_min_max == null)
                        {
                            _min_max = _factory.Create(_depth - 1,
                                _min_0, _middle_1, _middle_0, _max_1);
                        }
                        return _min_max[point];
                    }
                }
                else
                { // large side of dimension 0.
                    if (_middle_1 < point[1])
                    { // small side of dimension 1.
                        if (_max_min == null)
                        {
                            _max_min = _factory.Create(_depth - 1,
                                _middle_0, _min_1, _max_0, _middle_1);
                        }
                        return _max_min[point];

                    }
                    else
                    { // large side of dimension 1.
                        if (_max_max == null)
                        {
                            _max_max = _factory.Create(_depth - 1,
                                _middle_0, _middle_1, _max_0, _max_1);
                        }
                        return _max_max[point];
                    }
                }
            }
        }

        /// <summary>
        /// Returns the depot of this quad tree.
        /// </summary>
        public int Dept
        {
            get
            {
                return _depth;
            }
        }

        /// <summary>
        /// Returns the min of this quad tree in dimension 0.
        /// </summary>
        public double Min0
        {
            get
            {
                return _min_0;
            }
        }

        /// <summary>
        /// The max of this quad tree in dimension 0.
        /// </summary>
        public double Max0
        {
            get
            {
                return _min_0;
            }
        }

        /// <summary>
        /// Returns the min of this quad tree in dimension 1.
        /// </summary>
        public double Min1
        {
            get
            {
                return _min_1;
            }
        }

        /// <summary>
        /// The max of this quad tree in dimension 1.
        /// </summary>
        public double Max1
        {
            get
            {
                return _min_1;
            }
        }

        public QuadTreeNode<PointType> MinMin
        {
            get
            {
                return _min_min;
            }
        }

        public QuadTreeNode<PointType> MinMax
        {
            get
            {
                return _min_max;
            }
        }

        public QuadTreeNode<PointType> MaxMin
        {
            get
            {
                return _max_min;
            }
        }

        public QuadTreeNode<PointType> MaxMax
        {
            get
            {
                return _max_max;
            }
        }
    }
}
