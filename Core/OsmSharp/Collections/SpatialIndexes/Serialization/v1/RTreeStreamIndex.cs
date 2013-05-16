// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Math.Primitives;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v1
{
    /// <summary>
    /// R-tree implementation of a spatial index.
    /// http://en.wikipedia.org/wiki/R-tree
    /// 
    /// This tree reads it's data from a byte-stream and is serializable to a byte stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RTreeStreamIndex<T> : ISpatialIndex<T>
    {
        /// <summary>
        /// Holds the root node of the tree.
        /// </summary>
        private RTreeNodeBase _root;

        /// <summary>
        /// Holds the maximum leaf size M.
        /// </summary>
        private readonly int _maxLeafSize = 50;

        /// <summary>
        /// Holds the minimum leaf size m.
        /// </summary>
        private readonly int _minLeafSize = 20;

        /// <summary>
        /// Initializes a new instance of the OsmSharp.Collections.SpatialIndexes.RTreeIndex class.
        /// </summary>
        public RTreeStreamIndex()
        {
            _root = null;
        }

        /// <summary>
        /// Initializes a new instance of the OsmSharp.Collections.SpatialIndexes.RTreeIndex class.
        /// </summary>
        /// <param name="M">Holds the maximum leaf size M.</param>
        /// <param name="m">Holds the minimum leaf size m.</param>
        public RTreeStreamIndex(int M, int m)
        {
            _root = null;

            _maxLeafSize = M;
            _minLeafSize = m;
        }

        /// <summary>
        /// Returns the root node.
        /// </summary>
        internal RTreeNodeBase Root
        {
            get { return _root; }
        }

        /// <summary>
        /// Queries this index and returns all objects with overlapping bounding boxes.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(RectangleF2D box)
        {
            if (box == null)
                throw new ArgumentNullException("box");

            var result = new HashSet<T>();
            if (_root != null)
            {
                _root.Get(box, result);
            }
            return result;
        }

        /// <summary>
        /// Returns the count.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _root.CountLeaves();
        }

        /// <summary>
        /// Adds a new item with the corresponding box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="data"></param>
        public void Add(RectangleF2D box, T data)
        {
            if (box == null)
                throw new ArgumentNullException("box");

            // create the new leaf to be added.
            var leaf = new RTreeLeaf(data);
            if (_root == null)
            { // create a new root node.
                _root = new RTreeLeafNode();
            }

            // the root exists, search the correct leaf.
            RTreeLeafNode l = _root.ChooseLeaf(box);
            RTreeNode newRoot = l.Add(box, leaf, _maxLeafSize, _minLeafSize);
            if (newRoot != null)
            {
                _root = newRoot;
            }
        }

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {

        }


        /// <summary>
        /// R-tree node base class.
        /// </summary>
        internal abstract class RTreeNodeBase
        {
            /// <summary>
            /// Initializes a new instance of the OsmSharp.Collections.SpatialIndexes.RTreeIndex.RTreeNodeBase class.
            /// </summary>
            protected RTreeNodeBase()
            {

            }

            /// <summary>
            /// Returns true if this node is the root.
            /// </summary>
            public bool IsRoot
            {
                get { return this.Parent == null; }
            }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <value>The parent.</value>
            public RTreeNode Parent
            {
                get;
                set;
            }

            /// <summary>
            /// Returns the most tight box available.
            /// </summary>
            /// <returns></returns>
            protected internal abstract RectangleF2D GetBox();

            /// <summary>
            /// Chooses 
            /// </summary>
            /// <param name="box"></param>
            /// <returns></returns>
            internal abstract RTreeLeafNode ChooseLeaf(RectangleF2D box);
            
            /// <summary>
            /// Picks the seeds for the split-node procedure.
            /// </summary>
            /// <param name="boxes"></param>
            /// <returns>The seeds.</returns>
            protected static int[] PickSeeds(List<RectangleF2D> boxes)
            {
                // the Quadratic Split version: selecting the two items that waste the most space
                // if put together.

                var seeds = new int[2];
                double loss = 0;
                for (int idx1 = 0; idx1 < boxes.Count; idx1++)
                {
                    for (int idx2 = 0; idx2 < idx1; idx2++)
                    {
                        double localLoss = boxes[idx1].Union(boxes[idx2]).Surface -
                            boxes[idx1].Surface - boxes[idx2].Surface;
                        if (localLoss > loss)
                        {
                            loss = localLoss;
                            seeds[0] = idx1;
                            seeds[1] = idx2;
                        }
                    }
                }

                return seeds;
            }

            /// <summary>
            /// Picks the next item.
            /// </summary>
            /// <returns>The next.</returns>
            /// <param name="leafs">The leaves.</param>
            /// <param name="boxes">The boxes to choose from.</param>
            /// <param name="leafIdx"></param>
            protected static int PickNext(RectangleF2D[] leafs, IList<RectangleF2D> boxes, out int leafIdx)
            {
                double difference = double.MinValue;
                leafIdx = 0;
                int pickedIdx = -1;
                for (int idx = 0; idx < boxes.Count; idx++)
                {
                    RectangleF2D item = boxes[idx];
                    double d1 = item.Union(leafs[0]).Surface -
                                item.Surface;
                    double d2 = item.Union(leafs[1]).Surface -
                                item.Surface;

                    double localDifference = System.Math.Abs(d1 - d2);
                    if (difference < localDifference)
                    {
                        difference = localDifference;
                        if (d1 == d2)
                        {
                            leafIdx = (leafs[0].Surface < leafs[1].Surface) ? 0 : 1;
                        }
                        else
                        {
                            leafIdx = (d1 < d2) ? 0 : 1;
                        }
                        pickedIdx = idx;
                    }
                }
                return pickedIdx;
            }

            /// <summary>
            /// Fills the given hashset with objects having a box overlapping the given box.
            /// </summary>
            /// <param name="box"></param>
            /// <param name="result"></param>
            internal abstract void Get(RectangleF2D box, HashSet<T> result);

            /// <summary>
            /// Returns the total count of all leaves.
            /// </summary>
            /// <returns></returns>
            internal abstract int CountLeaves();
        }

        /// <summary>
        /// Represents a node in an R-tree that is not a leaf.
        /// </summary>
        internal class RTreeNode : RTreeNodeBase, IEnumerable<RTreeNodeBase>
        {
            /// <summary>
            /// Holds the boxes of the children in this node.
            /// </summary>
            private readonly List<RectangleF2D> _boxes;

            /// <summary>
            /// Holds the children of this node.
            /// </summary>
            private readonly List<RTreeNodeBase> _children;

            /// <summary>
            /// Creates a new R-tree node.
            /// </summary>
            public RTreeNode()
            {
                _boxes = new List<RectangleF2D>();
                _children = new List<RTreeNodeBase>();
            }

            /// <summary>
            /// Returns the numbers of children.
            /// </summary>
            public int Count
            {
                get { return _boxes.Count; }
            }

            /// <summary>
            /// Returns the child at the given index.
            /// </summary>
            /// <param name="idx"></param>
            /// <returns></returns>
            public RTreeNodeBase Child(int idx)
            {
                return _children[idx];
            }

            /// <summary>
            /// Returns the box at the given index.
            /// </summary>
            /// <param name="idx"></param>
            /// <returns></returns>
            public RectangleF2D Box(int idx)
            {
                return _boxes[idx];
            }

            /// <summary>
            /// Adds a new child with the given box and returns a new root if needed.
            /// </summary>
            /// <param name="box">The box of the child.</param>
            /// <param name="child">The child node.</param>
            /// <param name="M"></param>
            /// <param name="m"></param>
            internal RTreeNode Add(RectangleF2D box, RTreeNodeBase child, int M, int m)
            {
                if (box == null) throw new ArgumentNullException("box");
                if (child == null) throw new ArgumentNullException("child");

                RTreeNodeBase ll = null;
                if (_boxes.Count == M)
                { // split the node.
                    ll = this.Split(box, child, M, m);
                }
                else
                { // add the child.
                    child.Parent = this;
                    _boxes.Add(box);
                    _children.Add(child);
                }

                // adjust the tree.
                RTreeNode n = this;
                RTreeNodeBase nn = ll;
                while (!n.IsRoot)
                { // keep going until the root is reached.
                    RTreeNode p = n.Parent;
                    p.TightenFor(n); // tighten the parent box around n.


                    if (nn != null)
                    { // propagate split if needed.
                        if (p._boxes.Count == M)
                        { // parent needs to be split.
                            nn = p.Split(nn.GetBox(), nn, M, m);
                        }
                        else
                        { // add the other 'split' node.
                            p.Add(nn.GetBox(), nn, M, m);
                            nn = null;
                        }
                    }
                    n = p;
                }
                if (nn != null)
                { // create a new root node and 
                    var root = new RTreeNode();
                    root.Add(n.GetBox(), n, M, m);
                    root.Add(nn.GetBox(), nn, M, m);
                    return root;
                }
                return null; // no new root node needed.
            }

            /// <summary>
            /// Splits the current node and returns the second half.
            /// </summary>
            /// <param name="box"></param>
            /// <param name="child"></param>
            /// <param name="M"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            internal RTreeNodeBase Split(RectangleF2D box, RTreeNodeBase child, int M, int m)
            {
                // first add child then split.
                _boxes.Add(box);
                _children.Add(child);

                // split this node.
                int[] seeds = RTreeNode.PickSeeds(_boxes);

                var data = new List<KeyValuePair<RectangleF2D, RTreeNodeBase>>[] { 
                new List<KeyValuePair<RectangleF2D, RTreeNodeBase>>(),
                new List<KeyValuePair<RectangleF2D, RTreeNodeBase>>() };
                data[0].Add(new KeyValuePair<RectangleF2D, RTreeNodeBase>(_boxes[seeds[0]], _children[seeds[0]]));
                data[1].Add(new KeyValuePair<RectangleF2D, RTreeNodeBase>(_boxes[seeds[1]], _children[seeds[1]]));
                var boxes = new RectangleF2D[2] { _boxes[seeds[0]], _boxes[seeds[1]] };

                _boxes.RemoveAt(seeds[0]); // seeds[1] is always < seeds[0].
                _boxes.RemoveAt(seeds[1]);
                _children.RemoveAt(seeds[0]);
                _children.RemoveAt(seeds[1]);

                while (_boxes.Count > 0)
                {
                    // check if one of them needs em all!
                    if (data[0].Count + _boxes.Count == m)
                    { // all remaining boxes need te be assigned here.
                        for (int idx = 0; idx < _boxes.Count; idx++)
                        {
                            boxes[0] = boxes[0].Union(_boxes[0]);
                            data[0].Add(new KeyValuePair<RectangleF2D, RTreeNodeBase>(_boxes[0], _children[0]));

                            _boxes.RemoveAt(0);
                            _children.RemoveAt(0);
                        }
                    }
                    else if (data[1].Count + _boxes.Count == m)
                    { // all remaining boxes need te be assigned here.
                        for (int idx = 0; idx < _boxes.Count; idx++)
                        {
                            boxes[1] = boxes[1].Union(_boxes[0]);
                            data[1].Add(new KeyValuePair<RectangleF2D, RTreeNodeBase>(_boxes[0], _children[0]));

                            _boxes.RemoveAt(0);
                            _children.RemoveAt(0);
                        }
                    }
                    else
                    { // choose one of the leaves.
                        int leafIdx;
                        int nextId = RTreeNode.PickNext(boxes, _boxes, out leafIdx);

                        boxes[leafIdx] = boxes[leafIdx].Union(_boxes[nextId]);
                        data[leafIdx].Add(new KeyValuePair<RectangleF2D, RTreeNodeBase>(_boxes[nextId], _children[nextId]));

                        _boxes.RemoveAt(nextId);
                        _children.RemoveAt(nextId);
                    }
                }

                // set boxes on this node.
                _boxes.Clear();
                _children.Clear();
                for (int idx = 0; idx < data[0].Count; idx++)
                {
                    _boxes.Add(data[0][idx].Key);
                    _children.Add(data[0][idx].Value);
                    data[0][idx].Value.Parent = this;
                }

                // set boxes on other node.
                var other = new RTreeNode();
                for (int idx = 0; idx < data[1].Count; idx++)
                {
                    other._boxes.Add(data[1][idx].Key);
                    other._children.Add(data[1][idx].Value);
                    data[1][idx].Value.Parent = other;
                }
                return other;
            }

            /// <summary>
            /// Choose the leaf to best place the given box.
            /// </summary>
            /// <param name="box"></param>
            /// <returns></returns>
            internal override RTreeLeafNode ChooseLeaf(RectangleF2D box)
            {
                if (box == null) throw new ArgumentNullException("box");

                // check if there are children. If not just return this node.
                if (_boxes.Count == 0)
                {
                    throw new Exception("Node found without children!");
                }

                // find the best child.
                RTreeNodeBase bestChild = null;
                RectangleF2D bestBox = null;
                double bestIncrease = double.MaxValue;
                for (int idx = 0; idx < _boxes.Count; idx++)
                {
                        RectangleF2D union = _boxes[idx].Union(box);
                        double increase = union.Surface - _boxes[idx].Surface; // calculates the increase.
                        if (bestIncrease > increase)
                        {
                            // the increase for this child is smaller.
                            bestIncrease = increase;
                            bestChild = _children[idx];
                            bestBox = _boxes[idx];
                        }
                        else if (bestBox != null &&
                                 bestIncrease == increase)
                        {
                            // the increase is indentical, choose the smalles child.
                            if (_boxes[idx].Surface < bestBox.Surface)
                            {
                                bestChild = _children[idx];
                                bestBox = _boxes[idx];
                            }
                        }
                }
                if (bestChild == null)
                {
                    throw new Exception("Finding best child failed!");
                }
                // get leaf from best child.
                return bestChild.ChooseLeaf(box);
            }

            /// <summary>
            /// Tightens the boxes in this node.
            /// </summary>
            public void Tighten()
            {
                for (int idx = 0; idx < _boxes.Count; idx++)
                {
                    _boxes[idx] = _children[idx].GetBox();
                }
            }

            /// <summary>
            /// Tightens the box for the given child.
            /// </summary>
            /// <param name="child"></param>
            public void TightenFor(RTreeNodeBase child)
            {
                for (int idx = 0; idx < _boxes.Count; idx++)
                {
                    if (_children[idx] == child)
                    {
                        _boxes[idx] = _children[idx].GetBox();
                        return;
                    }
                }
            }

            /// <summary>
            /// Returns the most tight box available.
            /// </summary>
            /// <returns></returns>
            protected internal override RectangleF2D GetBox()
            {
                RectangleF2D box = _boxes[0];
                for (int idx = 1; idx < _boxes.Count; idx++)
                {
                    box = box.Union(_boxes[idx]);
                }
                return box;
            }

            /// <summary>
            /// Returns a enumerator.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<RTreeNodeBase> GetEnumerator()
            {
                return _children.GetEnumerator();
            }

            /// <summary>
            /// Returns a enumerator.
            /// </summary>
            /// <returns></returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _children.GetEnumerator();
            }

            /// <summary>
            /// Fills the given hashset with objects having a box overlapping the given box.
            /// </summary>
            /// <param name="box"></param>
            /// <param name="result"></param>
            internal override void Get(RectangleF2D box, HashSet<T> result)
            {
                for (int idx = 0; idx < _boxes.Count; idx++)
                {
                    if (_boxes[idx].Overlaps(box))
                    {
                        if (box.IsInside(_boxes[idx]))
                        { // all elements are inside the given box.
                            if (_children[idx] is RTreeNode)
                            {
                                (_children[idx] as RTreeNode).GetAll(result);
                            }
                            else if (_children[idx] is RTreeLeafNode)
                            {
                                (_children[idx] as RTreeLeafNode).GetAll(result);
                            }
                        }
                        else if (_children[idx] is RTreeNode)
                        { // return all child elements.
                            (_children[idx] as RTreeNode).Get(box, result);
                        }
                        else if (_children[idx] is RTreeLeafNode)
                        {
                            (_children[idx] as RTreeLeafNode).Get(box, result);
                        }
                    }
                }
            }

            /// <summary>
            /// Fills the given hashset with all objects in this node and all nodes below.
            /// </summary>
            internal void GetAll(HashSet<T> result)
            {
                for (int idx = 0; idx < _boxes.Count; idx++)
                {
                    if (_children[idx] is RTreeNode)
                    { // return all child elements.
                        (_children[idx] as RTreeNode).GetAll(result);
                    }
                    else if (_children[idx] is RTreeLeafNode)
                    {
                        (_children[idx] as RTreeLeafNode).GetAll(result);
                    }
                }
            }

            /// <summary>
            /// Returns the total count of all leaves.
            /// </summary>
            internal override int CountLeaves()
            {
                int leaves = 0;
                for (int idx = 0; idx < _boxes.Count; idx++)
                {
                    if (_children[idx] is RTreeNode)
                    { // return all child elements.
                        leaves = leaves + (_children[idx] as RTreeNode).CountLeaves();
                    }
                    else if (_children[idx] is RTreeLeafNode)
                    {
                        leaves = leaves + (_children[idx] as RTreeLeafNode).CountLeaves();
                    }
                }
                return leaves;
            }
        }

        /// <summary>
        /// Represents a leaf-node containing all the actual data.
        /// </summary>
        internal class RTreeLeafNode : RTreeNodeBase
        {
            /// <summary>
            /// Holds the boxes of the children in this node.
            /// </summary>
            private readonly List<RectangleF2D> _boxes;

            /// <summary>
            /// Holds the children of this node.
            /// </summary>
            private readonly List<RTreeLeaf> _children;

            /// <summary>
            /// Creates a new leaf node.
            /// </summary>
            public RTreeLeafNode()
            {
                _boxes = new List<RectangleF2D>();
                _children = new List<RTreeLeaf>();
            }

            /// <summary>
            /// Adds a new leaf to this node.
            /// </summary>
            /// <param name="box"></param>
            /// <param name="leaf"></param>
            /// <param name="M"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            internal RTreeNode Add(RectangleF2D box, RTreeLeaf leaf, int M, int m)
            {
                if (box == null) throw new ArgumentNullException("box");

                RTreeLeafNode ll = null;
                if (_boxes.Count == M)
                { // split the node.
                    ll = this.Split(box, leaf, M, m);
                }
                else
                { // add the child.
                    _boxes.Add(box);
                    _children.Add(leaf);
                }

                // adjust the tree.
                RTreeNodeBase n = this;
                RTreeNodeBase nn = ll;
                while (!n.IsRoot)
                { // keep going until the root is reached.
                    RTreeNode p = n.Parent;
                    p.TightenFor(n); // tighten the parent box around n.

                    if (nn != null)
                    { // propagate split if needed.
                        if (p.Count == M)
                        { // parent needs to be split.
                            nn = p.Split(nn.GetBox(), nn, M, m);
                        }
                        else
                        { // add the other 'split' node.
                            p.Add(nn.GetBox(), nn, M, m);
                            nn = null;
                        }
                    }
                    n = p;
                }
                if (nn != null)
                { // create a new root node and 
                    var root = new RTreeNode();
                    root.Add(n.GetBox(), n, M, m);
                    root.Add(nn.GetBox(), nn, M, m);
                    return root;
                }
                return null; // no new root node needed.
            }

            /// <summary>
            /// Splits the current node and returns the second half.
            /// </summary>
            /// <param name="box"></param>
            /// <param name="leaf"></param>
            /// <param name="M"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            internal RTreeLeafNode Split(RectangleF2D box, RTreeLeaf leaf, int M, int m)
            {
                // first add child then split.
                _boxes.Add(box);
                _children.Add(leaf);

                // split this node.
                int[] seeds = RTreeNode.PickSeeds(_boxes);

                var data = new List<KeyValuePair<RectangleF2D, RTreeLeaf>>[] { 
                new List<KeyValuePair<RectangleF2D, RTreeLeaf>>(),
                new List<KeyValuePair<RectangleF2D, RTreeLeaf>>() };
                data[0].Add(new KeyValuePair<RectangleF2D, RTreeLeaf>(_boxes[seeds[0]], 
                    _children[seeds[0]]));
                data[1].Add(new KeyValuePair<RectangleF2D, RTreeLeaf>(_boxes[seeds[1]], 
                    _children[seeds[1]]));
                var boxes = new RectangleF2D[2] { _boxes[seeds[0]], _boxes[seeds[1]] };

                _boxes.RemoveAt(seeds[0]); // seeds[1] is always < seeds[0].
                _boxes.RemoveAt(seeds[1]);
                _children.RemoveAt(seeds[0]);
                _children.RemoveAt(seeds[1]);

                while (_boxes.Count > 0)
                {
                    // check if one of them needs em all!
                    if (data[0].Count + _boxes.Count == m)
                    { // all remaining boxes need te be assigned here.
                        for (int idx = 0; idx < _boxes.Count; idx++)
                        {
                            boxes[0] = boxes[0].Union(_boxes[0]);
                            data[0].Add(new KeyValuePair<RectangleF2D, RTreeLeaf>(_boxes[0], 
                                _children[0]));

                            _boxes.RemoveAt(0);
                            _children.RemoveAt(0);
                        }
                    }
                    else if (data[1].Count + _boxes.Count == m)
                    { // all remaining boxes need te be assigned here.
                        for (int idx = 0; idx < _boxes.Count; idx++)
                        {
                            boxes[1] = boxes[1].Union(_boxes[0]);
                            data[1].Add(new KeyValuePair<RectangleF2D, RTreeLeaf>(_boxes[0], 
                                _children[0]));

                            _boxes.RemoveAt(0);
                            _children.RemoveAt(0);
                        }
                    }
                    else
                    { // choose one of the leaves.
                        int leafIdx;
                        int nextId = RTreeNode.PickNext(boxes, _boxes, out leafIdx);

                        boxes[leafIdx] = boxes[leafIdx].Union(_boxes[nextId]);
                        data[leafIdx].Add(new KeyValuePair<RectangleF2D, RTreeLeaf>(_boxes[nextId], 
                            _children[nextId]));

                        _boxes.RemoveAt(nextId);
                        _children.RemoveAt(nextId);
                    }
                }

                // set boxes on this node.
                _boxes.Clear();
                _children.Clear();
                for (int idx = 0; idx < data[0].Count; idx++)
                {
                    _boxes.Add(data[0][idx].Key);
                    _children.Add(data[0][idx].Value);
                }

                // set boxes on other node.
                var other = new RTreeLeafNode();
                for (int idx = 0; idx < data[1].Count; idx++)
                {
                    other._boxes.Add(data[1][idx].Key);
                    other._children.Add(data[1][idx].Value);
                }
                return other;
            }

            /// <summary>
            /// Returns the leaf count.
            /// </summary>
            /// <returns></returns>
            internal override int CountLeaves()
            {
                return _boxes.Count;
            }

            /// <summary>
            /// Returns all data in this ndoe.
            /// </summary>
            /// <param name="result"></param>
            internal void GetAll(HashSet<T> result)
            {
                foreach (var data in _children)
                {
                    result.Add(data.Data);
                }
            }

            /// <summary>
            /// Returns all data that overlaps the given box.
            /// </summary>
            /// <param name="box"></param>
            /// <param name="result"></param>
            internal override void Get(RectangleF2D box, HashSet<T> result)
            {
                for (int idx = 0; idx < _boxes.Count; idx++)
                {
                    if (_boxes[idx].Overlaps(box))
                    {
                        result.Add(_children[idx].Data);
                    }
                }
            }

            /// <summary>
            /// Returns the most tight box available.
            /// </summary>
            /// <returns></returns>
            protected internal override RectangleF2D GetBox()
            {
                RectangleF2D box = _boxes[0];
                for (int idx = 1; idx < _boxes.Count; idx++)
                {
                    box = box.Union(_boxes[idx]);
                }
                return box;
            }

            /// <summary>
            /// Chooses a good leaf.
            /// </summary>
            /// <param name="box"></param>
            /// <returns></returns>
            internal override RTreeLeafNode ChooseLeaf(RectangleF2D box)
            { // when called, this node was already choosen.
                return this;
            }
        }

        /// <summary>
        /// Represents a leaf in an R-tree.
        /// </summary>
        internal class RTreeLeaf
        {
            /// <summary>
            /// Creates a new R-tree leaf.
            /// </summary>
            /// <param name="data"></param>
            public RTreeLeaf(T data)
            {
                //this.Box = box;
                this.Data = data;
            }

            /// <summary>
            /// Gets the data in this leaf.
            /// </summary>
            public T Data { get; private set; }
        }
    }
}