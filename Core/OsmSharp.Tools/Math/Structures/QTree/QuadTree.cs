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

namespace OsmSharp.Tools.Math.Structures.QTree
{
    /// <summary>
    /// Interface to implement a factory to create a quad tree.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    /// <typeparam name="DataType"></typeparam>
    /// <remarks>
    /// This quadtree implementation can be a lot better:
    /// - Seperate the structure from the primitives and implement faster bounding box overlapping code.
    /// - Use an indentifier for each leaf node, removing the need to keep data at each node.
    /// </remarks>
    public class QuadTree<PointType, DataType> : ILocatedObjectIndex<PointType, DataType>
        where PointType : PointF2D
    {
        /// <summary>
        /// The root node of this quad tree.
        /// </summary>
        private QuadTreeNode _root;

        /// <summary>
        /// Creates a rootless quad tree.
        /// </summary>
        public QuadTree()
        {
            _root = null;
        }

        /// <summary>
        /// Creates a new quad tree.
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public QuadTree(
            int dept, GenericRectangleF2D<PointType> bounds)
        {
            _root = new QuadTreeNode(dept, bounds);
        }

        /// <summary>
        /// Creates a new quad tree.
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="min_0"></param>
        /// <param name="min_1"></param>
        /// <param name="max_0"></param>
        /// <param name="max_1"></param>
        /// <returns></returns>
        public QuadTree(
            int dept, double min_0, double min_1, double max_0, double max_1)
        {
            _root = new QuadTreeNode(dept, min_0, min_1, max_0, max_1);
        }

        /// <summary>
        /// Returns the smallest quadtree at depth zero.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private QuadTreeNode GetOrCreateAt(PointType point)
        {
            if (_root == null)
            { // create a default root.
                _root = new QuadTreeNode(0,
                    point[0] - 0.01, point[1] - 0.01, point[0] + 0.01, point[1] + 0.01);
            }

            if (!_root.IsInsideBox(point))
            { // the point is inside.
                if (point[0] < _root.Min0)
                { // expand towards the minimum.
                    // descide to the minimum or maximum.
                    if (point[1] < _root.Min1)
                    { // expand towards the minimum.
                        _root = new QuadTreeNode(true, true,
                            _root);

                        // recursive call to the quad tree.
                        return this.GetOrCreateAt(point);
                    }
                    else
                    { // expand towards the maximum.
                        _root = new QuadTreeNode(true, false,
                            _root);

                        // recursive call to the quad tree.
                        return this.GetOrCreateAt(point);
                    }
                }
                if (point[0] > _root.Max0)
                { // expand towards the maximum.
                    // descide to the minimum or maximum.
                    if (point[1] < _root.Min1)
                    { // expand towards the minimum.
                        _root = new QuadTreeNode(false, true,
                            _root);

                        // recursive call to the quad tree.
                        return this.GetOrCreateAt(point);
                    }
                    else
                    { // expand towards the maximum.
                        _root = new QuadTreeNode(false, false,
                            _root);

                        // recursive call to the quad tree.
                        return this.GetOrCreateAt(point);
                    }
                }
                if (point[1] < _root.Min1)
                { // expand towards the minimum.
                    // expand towards the maximum.
                    _root = new QuadTreeNode(false, true,
                        _root);

                    // recursive call to the quad tree.
                    return this.GetOrCreateAt(point);
                }
                if (point[1] > _root.Max1)
                { // expand towards the maximum.// expand towards the maximum.
                    _root = new QuadTreeNode(false, false,
                        _root);

                    // recursive call to the quad tree.
                    return this.GetOrCreateAt(point);
                }
                throw new Exception("The point is not in the route but not outside of any bound!?");
            }
            return _root.GetOrCreateAt(point);
        }

        /// <summary>
        /// Returns all data inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IEnumerable<DataType> GetInside(GenericRectangleF2D<PointType> box)
        {
            if (_root == null)
            { // this can only mean not data was added yet to this index.
                // return an empty enumerable.
                return new List<DataType>();
            }

            // there is data!
            List<DataType> data = new List<DataType>();
            _root.AddInsideAtNode(data, _root, box);
            return data;
        }

        /// <summary>
        /// Adds new data at a given location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="data"></param>
        public void Add(PointType location, DataType data)
        {
            QuadTreeNode leaf_node = this.GetOrCreateAt(
                location);

            leaf_node.AddData(location, data);
        }

        /// <summary>
        /// An implementation of a quad tree.
        /// </summary>
        private class QuadTreeNode
        {
            /// <summary>
            /// The bottom left child quad tree.
            /// </summary>
            private QuadTreeNode _min_min;

            /// <summary>
            /// The bottom right child quad tree.
            /// </summary>
            private QuadTreeNode _min_max;

            /// <summary>
            /// The top right child quad tree.
            /// </summary>
            private QuadTreeNode _max_min;

            /// <summary>
            /// The top left child quad tree.
            /// </summary>
            private QuadTreeNode _max_max;

            /// <summary>
            /// The bounds of this node.
            /// </summary>
            private RectangleF2D _bounds;

            /// <summary>
            /// The middle of this node in dimension 0.
            /// </summary>
            private double _middle_0;

            /// <summary>
            /// The middle of this node in dimension 1.
            /// </summary>
            private double _middle_1;

            /// <summary>
            /// The dept of this quad tree.
            /// </summary>
            private int _depth;

            /// <summary>
            /// The data in this node.
            /// </summary>
            private List<KeyValuePair<PointType, DataType>> _data;

            /// <summary>
            /// Creates a quad tree with given bounds.
            /// </summary>
            /// <param name="dept"></param>
            /// <param name="bounds"></param>
            public QuadTreeNode(int dept, GenericRectangleF2D<PointType> bounds)
            {
                _depth = dept;

                _bounds = new RectangleF2D(new PointF2D(bounds.Max[0], bounds.Max[1]),
                    new PointF2D(bounds.Min[0], bounds.Min[1]));
                //_max_0 = bounds.Max[0]; // max x = right.
                //_max_1 = bounds.Max[1]; // max y = top.

                //_min_0 = bounds.Min[0]; // min y = bottom.
                //_min_1 = bounds.Min[1]; // min x = left.

                // calculate the middles.
                _middle_0 = (bounds.Min[0] + bounds.Max[0]) / 2.0;
                _middle_1 = (bounds.Min[1] + bounds.Max[1]) / 2.0;

                if (_depth == 0)
                {
                    _data = new List<KeyValuePair<PointType, DataType>>();
                }
            }

            /// <summary>
            /// Creates a quad tree with given bounds.
            /// </summary>
            /// <param name="dept"></param>
            /// <param name="min_0"></param>
            /// <param name="min_1"></param>
            /// <param name="max_0"></param>
            /// <param name="max_1"></param>
            public QuadTreeNode(int dept, double min_0, double min_1, double max_0, double max_1)
            {
                _depth = dept;

                _bounds = new RectangleF2D(new PointF2D(max_0, max_1),
                    new PointF2D(min_0, min_1));
                //_max_1 = max_1; // max y = top.
                //_max_0 = max_0; // max x = right.

                //_min_0 = min_0; // min y = bottom.
                //_min_1 = min_1; // min x = left.

                // calculate the middles.
                _middle_0 = (min_0 + max_0) / 2.0;
                _middle_1 = (min_1 + max_1) / 2.0;

                if (_depth == 0)
                {
                    _data = new List<KeyValuePair<PointType, DataType>>();
                }
            }

            /// <summary>
            /// Creates a quad tree with given bounds.
            /// </summary>
            /// <param name="min0"></param>
            /// <param name="min1"></param>
            /// <param name="node"></param>
            public QuadTreeNode(bool min0, bool min1,
                QuadTreeNode node)
            {
                _depth = node.Depth + 1;

                double min_0, max_0, min_1, max_1;

                double diff_0 = node.Max0 - node.Min0;
                double diff_1 = node.Max1 - node.Min1;

                if (min0)
                {
                    min_0 = node.Min0 - diff_0;
                    max_0 = node.Max0;

                    if (min1)
                    {
                        min_1 = node.Min1 - diff_1;
                        max_1 = node.Max1;

                        _max_max = node;
                    }
                    else
                    {
                        min_1 = node.Min1;
                        max_1 = node.Max1 + diff_1;

                        _max_min = node;
                    }
                }
                else
                {
                    if (min1)
                    {
                        min_1 = node.Min1 - diff_1;
                        max_1 = node.Max1;

                        _min_max = node;
                    }
                    else
                    {
                        min_1 = node.Min1;
                        max_1 = node.Max1 + diff_1;

                        _min_min = node;
                    }
                    min_0 = node.Min0;
                    max_0 = node.Max0 + diff_0;
                }

                // calculate the middles.
                _middle_0 = (min_0 + max_0) / 2.0;
                _middle_1 = (min_1 + max_1) / 2.0;

                _bounds = new RectangleF2D(new PointF2D(max_0, max_1),
                    new PointF2D(min_0, min_1));

                if (_depth == 0)
                {
                    _data = new List<KeyValuePair<PointType, DataType>>();
                }
            }

            /// <summary>
            /// Returns the smallest quadtree node at depth zero that encompasses the given node.
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public QuadTreeNode GetOrCreateAt(PointType point)
            {
                // return this tree if this one is at dimension 0.
                if (_depth == 0)
                {
                    return this;
                }

                if (_middle_0 > point[0])
                { // small side of dimension 0.
                    if (_middle_1 > point[1])
                    { // small side of dimension 1.
                        if (_min_min == null)
                        {
                            _min_min = new QuadTreeNode(_depth - 1,
                                this.Min0, this.Min1, _middle_0, _middle_1);
                        }
                        return _min_min.GetOrCreateAt(point);
                    }
                    else
                    { // large side of dimension 1.
                        if (_min_max == null)
                        {
                            _min_max = new QuadTreeNode(_depth - 1,
                                this.Min0, _middle_1, _middle_0, this.Max1);
                        }
                        return _min_max.GetOrCreateAt(point);
                    }
                }
                else
                { // large side of dimension 0.
                    if (_middle_1 > point[1])
                    { // small side of dimension 1.
                        if (_max_min == null)
                        {
                            _max_min = new QuadTreeNode(_depth - 1,
                                _middle_0, this.Min1, this.Max0, _middle_1);
                        }
                        return _max_min.GetOrCreateAt(point);

                    }
                    else
                    { // large side of dimension 1.
                        if (_max_max == null)
                        {
                            _max_max = new QuadTreeNode(_depth - 1,
                                _middle_0, _middle_1, this.Max0, this.Max1);
                        }
                        return _max_max.GetOrCreateAt(point);
                    }
                }
            }

            /// <summary>
            /// Adds all the data in the given node and inside the given bounding box to the given data list.
            /// </summary>
            /// <param name="data"></param>
            /// <param name="node"></param>
            /// <param name="box"></param>
            public void AddInsideAtNode(IList<DataType> data, QuadTreeNode node, GenericRectangleF2D<PointType> box)
            {
                if (box.Overlaps(_bounds))
                { // ok there is an overlap.
                    if (_depth > 0)
                    {
                        if (_min_min != null)
                        {
                            _min_min.AddInsideAtNode(data, node, box);
                        }
                        if (_min_max != null)
                        {
                            _min_max.AddInsideAtNode(data, node, box);
                        }
                        if (_max_min != null)
                        {
                            _max_min.AddInsideAtNode(data, node, box);
                        }
                        if (_max_max != null)
                        {
                            _max_max.AddInsideAtNode(data, node, box);
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<PointType, DataType> data_pair in _data)
                        {
                            if (box.IsInside(data_pair.Key))
                            {
                                data.Add(data_pair.Value);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Returns the depht of this quad tree.
            /// </summary>
            public int Depth
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
                    return _bounds.Min[0];
                }
            }

            /// <summary>
            /// The max of this quad tree in dimension 0.
            /// </summary>
            public double Max0
            {
                get
                {
                    return _bounds.Max[0];
                }
            }

            /// <summary>
            /// Returns the min of this quad tree in dimension 1.
            /// </summary>
            public double Min1
            {
                get
                {
                    return _bounds.Min[1];
                }
            }

            /// <summary>
            /// The max of this quad tree in dimension 1.
            /// </summary>
            public double Max1
            {
                get
                {
                    return _bounds.Max[1];
                }
            }

            /// <summary>
            /// The minmin node.
            /// </summary>
            public QuadTreeNode MinMin
            {
                get
                {
                    return _min_min;
                }
            }

            /// <summary>
            /// The minmax node.
            /// </summary>
            public QuadTreeNode MinMax
            {
                get
                {
                    return _min_max;
                }
            }

            /// <summary>
            /// The maxmin node.
            /// </summary>
            public QuadTreeNode MaxMin
            {
                get
                {
                    return _max_min;
                }
            }

            /// <summary>
            /// The maxmax node.
            /// </summary>
            public QuadTreeNode MaxMax
            {
                get
                {
                    return _max_max;
                }
            }

            /// <summary>
            /// Adds data to this node.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="data"></param>
            internal void AddData(PointType point, DataType data)
            {
                if (_depth > 0)
                {
                    throw new Exception("Cannot add data to a non-leaf node!");
                }
                _data.Add(new KeyValuePair<PointType, DataType>(point, data));
            }

            /// <summary>
            /// Returns true if the point is inside.
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            internal bool IsInsideBox(PointType point)
            {
                return _bounds.IsInside(point);
            }
        }
    }
}