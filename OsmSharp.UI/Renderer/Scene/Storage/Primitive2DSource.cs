// OsmSharp - OpenStreetMap (OSM) SDK
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

using OsmSharp.Collections;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Collections.Tags;
using OsmSharp.UI.Renderer.Primitives;
using System.Collections.Generic;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// A primitives source based on serialized R-Trees.
    /// </summary>
    public class Primitive2DSource : IPrimitives2DSource
    {
        /// <summary>
        /// Holds the zoom factors.
        /// </summary>
        private float[] _zoomFactors;

        /// <summary>
        /// Holds the r-tree sources.
        /// </summary>
        private ISpatialIndexReadonly<Primitive2D>[] _rTrees;

        /// <summary>
        /// Creates a new primitives sources.
        /// </summary>
        /// <param name="zoomFactors"></param>
        /// <param name="rTrees"></param>
        internal Primitive2DSource(float[] zoomFactors, ISpatialIndexReadonly<Primitive2D>[] rTrees)
        {
            _zoomFactors = zoomFactors;
            _rTrees = rTrees;
        }

        /// <summary>
        /// Returns all objects visible for the given parameters.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="zoomFactor"></param>
        /// <returns></returns>
        public IEnumerable<Primitive2D> Get(View2D view, float zoomFactor)
        {
            ISpatialIndexReadonly<Primitive2D> sceneAtZoom = _rTrees[0];
            // find the part of this scene containing the objects for the requested zoom.
            for (int idx = 1; idx < _zoomFactors.Length; idx++)
            {
                if (zoomFactor <= _zoomFactors[idx])
                {
                    sceneAtZoom = _rTrees[idx];
                }
                else
                {
                    break;
                }
            }

            if (sceneAtZoom != null)
            {
                var primitives = new SortedSet<Primitive2D>(LayerComparer.GetInstance());
                foreach (var primitive in sceneAtZoom.Get(view.OuterBox))
                {
                    primitives.Add(primitive);
                }
                return primitives;
            }
            return new List<Primitive2D>();
        }

        /// <summary>
        /// Called an ongoing request has to be cancelled.
        /// </summary>
        public void GetCancel()
        {
            foreach (var tree in _rTrees)
            {
                tree.GetCancel();
            }
        }

        /// <summary>
        /// Returns the backcolor if any.
        /// </summary>
        public int? BackColor
        {
            get { return null; }
        }


        /// <summary>
        /// Layer comparer to sort objects by layer.
        /// </summary>
        private class LayerComparer : IComparer<Primitive2D>
        {
            private static LayerComparer _instance = null;

            public static LayerComparer GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new LayerComparer();
                }
                return _instance;
            }

            public int Compare(Primitive2D x, Primitive2D y)
            {
                if (x.Layer == y.Layer)
                { // objects with same layer, assume different.
                    return -1;
                }
                return x.Layer.CompareTo(y.Layer);
            }
        }
    }
}